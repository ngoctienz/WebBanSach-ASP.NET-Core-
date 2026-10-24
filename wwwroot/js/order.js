/* =========================================================
   Order Modal & Form – trung tâm chuyển đổi
   - Mọi nút [data-open-order] mở CÙNG một <dialog>
   - Validate phía client, tính tổng tiền
   - POST tới backend; CHỈ báo thành công khi server trả orderId
   - Sticky CTA mobile hiện khi CTA Hero ra khỏi màn hình
   ========================================================= */
(function () {
  'use strict';

  var NGK = window.NGK;
  var data = NGK.data;
  var product = data.product;
  var shipping = data.shipping || {};
  var orderCfg = data.order || {};
  var fmt = NGK.formatVND;
  var esc = NGK.escapeHTML;

  var dialog = document.getElementById('order-dialog');
  var form = document.getElementById('order-form');
  var result = document.getElementById('order-result');
  var statusEl = document.getElementById('order-status');
  var submitBtn = form.querySelector('[data-submit]');
  var submitLabel = form.querySelector('[data-submit-label]');
  var qtyInput = form.elements.quantity;
  var provinceSelect = form.elements.province;

  var lastTrigger = null;
  var submitting = false;
  var triedSubmit = false;
  var idempotencyKey = newKey();

  /* ---------- Khởi tạo nội dung form ---------- */
  if (provinceSelect && data.provinces) {
    data.provinces.forEach(function (name) {
      var opt = document.createElement('option');
      opt.value = name;
      opt.textContent = name;
      provinceSelect.appendChild(opt);
    });
  }
  qtyInput.max = product.maxQuantity;
  setText('[data-max-qty]', product.maxQuantity);
  setText('[data-product-edition]', product.edition);
  setText('[data-shipping-policy]', shipping.policy || '');

  function setText(selector, value) {
    dialog.querySelectorAll(selector).forEach(function (el) { el.textContent = value; });
  }

  function newKey() {
    if (window.crypto && crypto.randomUUID) return crypto.randomUUID();
    return 'k-' + Date.now() + '-' + Math.random().toString(16).slice(2);
  }

  /* ---------- Mở / đóng modal ---------- */
  function openOrderModal(trigger) {
    lastTrigger = trigger || null;
    if (dialog.open) return;

    // Sau một đơn thành công, lần mở tiếp theo là đơn mới
    if (!result.hidden) resetToForm(true);

    dialog.showModal();
    document.body.classList.add('modal-open');
    document.documentElement.style.overflow = 'hidden';
    form.elements.fullName.focus();
  }
  NGK.openOrderModal = openOrderModal;

  document.addEventListener('click', function (e) {
    var trigger = e.target.closest('[data-open-order]');
    if (!trigger) return;
    e.preventDefault();
    openOrderModal(trigger);
  });

  dialog.querySelectorAll('[data-close-order]').forEach(function (btn) {
    btn.addEventListener('click', function () { dialog.close(); });
  });

  // Click overlay để đóng (dữ liệu đã nhập vẫn được giữ)
  var pressedOnBackdrop = false;
  dialog.addEventListener('mousedown', function (e) { pressedOnBackdrop = e.target === dialog; });
  dialog.addEventListener('click', function (e) {
    if (e.target === dialog && pressedOnBackdrop && !submitting) dialog.close();
    pressedOnBackdrop = false;
  });

  // Không cho đóng bằng Escape khi đang gửi đơn
  dialog.addEventListener('cancel', function (e) { if (submitting) e.preventDefault(); });

  dialog.addEventListener('close', function () {
    document.body.classList.remove('modal-open');
    document.documentElement.style.overflow = '';
    if (lastTrigger && document.contains(lastTrigger)) lastTrigger.focus();
  });

  /* ---------- Số lượng & tổng tiền ---------- */
  function getQty() {
    var q = parseInt(qtyInput.value, 10);
    return isNaN(q) ? 0 : q;
  }

  function shippingFee(subtotal) {
    if (typeof shipping.freeFrom === 'number' && subtotal >= shipping.freeFrom) return 0;
    return typeof shipping.fee === 'number' ? shipping.fee : 0;
  }

  function updateSummary() {
    var qty = getQty();
    var validQty = qty >= 1 && qty <= product.maxQuantity;
    var subtotal = validQty && typeof product.price === 'number' ? product.price * qty : null;
    var ship = subtotal === null ? null : shippingFee(subtotal);

    setText('[data-sum="unit"]', fmt(product.price));
    setText('[data-sum="qty"]', validQty ? qty : '—');
    setText('[data-sum="subtotal"]', subtotal === null ? '—' : fmt(subtotal));
    setText('[data-sum="shipping"]',
      ship === 0 ? 'Miễn phí'
        : ship === null ? 'Báo sau khi xác nhận'
        : fmt(ship));
    setText('[data-sum="total"]',
      subtotal === null ? '—' : fmt(subtotal + (ship || 0)) + (ship === null ? ' + phí ship' : ''));

    form.querySelector('[data-qty="-1"]').disabled = qty <= 1;
    form.querySelector('[data-qty="1"]').disabled = qty >= product.maxQuantity;
  }

  form.querySelectorAll('[data-qty]').forEach(function (btn) {
    btn.addEventListener('click', function () {
      var next = getQty() + parseInt(btn.getAttribute('data-qty'), 10);
      qtyInput.value = Math.min(Math.max(next, 1), product.maxQuantity);
      updateSummary();
      if (triedSubmit) validateField('quantity');
    });
  });
  qtyInput.addEventListener('input', updateSummary);
  if (provinceSelect) {
    provinceSelect.addEventListener('change', updateSummary);
  }

  /* ---------- Validation ---------- */
  function normalizePhone(raw) {
    var p = String(raw || '').replace(/[\s.\-()]/g, '');
    if (p.indexOf('+84') === 0) p = '0' + p.slice(3);
    else if (p.indexOf('84') === 0 && p.length === 11) p = '0' + p.slice(2);
    return p;
  }

  var validators = {
    fullName: function (v) {
      v = (v || '').trim();
      if (!v) return 'Vui lòng nhập họ và tên.';
      if (v.length < 2 || v.length > 100) return 'Họ tên cần từ 2 đến 100 ký tự.';
      return '';
    },
    phone: function (v) {
      if (!(v || '').trim()) return 'Vui lòng nhập số điện thoại.';
      if (!/^0(3|5|7|8|9)\d{8}$/.test(normalizePhone(v))) return 'Số điện thoại chưa đúng định dạng Việt Nam (VD: 0912 345 678).';
      return '';
    },
    address: function (v) {
      v = (v || '').trim();
      if (!v) return 'Vui lòng nhập địa chỉ nhận hàng.';
      if (v.length < 5) return 'Địa chỉ quá ngắn, vui lòng ghi rõ địa chỉ nhận hàng.';
      return '';
    },
    quantity: function (v) {
      var q = Number(v);
      if (!Number.isInteger(q) || q < 1) return 'Số lượng tối thiểu là 1.';
      if (q > product.maxQuantity) return 'Số lượng tối đa là ' + product.maxQuantity + ' cuốn/đơn.';
      return '';
    }
  };

  function validateField(name) {
    var field = form.elements[name];
    if (!field || !validators[name]) return true;
    var msg = validators[name](field.value);
    var err = document.getElementById('e-' + name);
    field.setAttribute('aria-invalid', msg ? 'true' : 'false');
    if (err) {
      err.innerHTML = msg ? '<i class="fa-solid fa-circle-exclamation" aria-hidden="true"></i> ' + esc(msg) : '';
      err.classList.toggle('is-visible', !!msg);
    }
    return !msg;
  }

  Object.keys(validators).forEach(function (name) {
    var field = form.elements[name];
    if (!field) return;
    field.addEventListener('blur', function () { if (triedSubmit || field.value) validateField(name); });
    field.addEventListener('input', function () { if (triedSubmit) validateField(name); });
    field.addEventListener('change', function () { if (triedSubmit) validateField(name); });
  });

  /* ---------- Trạng thái ---------- */
  function showStatus(type, html) {
    statusEl.className = 'form-status mt-4' + (type ? ' is-' + type : '');
    statusEl.innerHTML = html || '';
    if (html) statusEl.scrollIntoView({ block: 'nearest', behavior: 'smooth' });
  }

  /* ---------- CSS Alert / Toast thông báo từ server ở góc trên bên phải (Tự tắt sau 5s) ---------- */
  function showCssAlert(type, message, durationMs) {
    durationMs = typeof durationMs === 'number' ? durationMs : 5000;
    
    // Đảm bảo nổi lên trên cùng ngay cả khi <dialog> đang mở dạng showModal
    var parentTarget = (dialog && dialog.open) ? dialog : document.body;
    var container = parentTarget.querySelector('.toast-alert-container');
    if (!container) {
      container = document.createElement('div');
      container.className = 'toast-alert-container';
      parentTarget.appendChild(container);
    }

    var alertEl = document.createElement('div');
    alertEl.className = 'toast-alert is-' + (type === 'success' ? 'success' : 'error');
    alertEl.setAttribute('role', 'alert');
    alertEl.innerHTML =
      '<i class="fa-solid ' + (type === 'success' ? 'fa-circle-check' : 'fa-circle-exclamation') + ' text-xl shrink-0" aria-hidden="true"></i>' +
      '<div class="flex-1 text-sm font-medium leading-snug">' + esc(message) + '</div>' +
      '<button type="button" class="ml-2 text-white/80 hover:text-white font-bold text-xl leading-none transition-colors" aria-label="Đóng">&times;</button>' +
      '<div class="toast-alert-progress" style="animation-duration: ' + (durationMs / 1000) + 's"></div>';

    var closeBtn = alertEl.querySelector('button');
    function dismiss() {
      alertEl.classList.remove('show');
      setTimeout(function () {
        if (alertEl.parentNode) alertEl.remove();
      }, 400);
    }
    closeBtn.addEventListener('click', dismiss);

    container.appendChild(alertEl);
    setTimeout(function () {
      alertEl.classList.add('show');
    }, 20);

    setTimeout(dismiss, durationMs);
  }

  /* ---------- Spinner & Trạng thái nút submit ---------- */
  function setLoading(on) {
    submitting = on;
    submitBtn.disabled = on;
    form.setAttribute('aria-busy', on ? 'true' : 'false');
    if (on) {
      var icon = submitBtn.querySelector('i, .spinner');
      if (icon) {
        icon.outerHTML = '<span class="spinner mr-2" aria-hidden="true"></span>';
      }
      submitLabel.textContent = 'Đang xử lý đơn hàng…';
      showStatus('info', '<span class="spinner mr-2" aria-hidden="true"></span><span>Hệ thống đang xử lý đơn hàng, vui lòng đợi…</span>');
    } else {
      var spin = submitBtn.querySelector('.spinner');
      if (spin) {
        spin.outerHTML = '<i class="fa-solid fa-lock" aria-hidden="true"></i>';
      }
      submitLabel.textContent = 'Xác nhận đặt hàng';
    }
  }

  /* ---------- Submit ---------- */
  form.addEventListener('submit', function (e) {
    e.preventDefault();
    if (submitting) return; // chống bấm nhiều lần
    triedSubmit = true;
    showStatus('', '');

    var firstInvalid = null;
    Object.keys(validators).forEach(function (name) {
      if (!validateField(name) && !firstInvalid) firstInvalid = form.elements[name];
    });
    if (firstInvalid) {
      firstInvalid.focus();
      showStatus('error', '<i class="fa-solid fa-circle-exclamation mt-0.5" aria-hidden="true"></i><span>Vui lòng kiểm tra lại các thông tin được đánh dấu.</span>');
      return;
    }

    var addrVal = form.elements.address ? form.elements.address.value.trim() : '';
    var payload = {
      tenKhachHang: form.elements.fullName.value.trim(),
      sdt: normalizePhone(form.elements.phone.value),
      diaChi: addrVal,
      soLuong: getQty(),
      maHinhThucThanhToan: form.elements.paymentMethod.value,
      ghiChu: form.elements.note.value.trim().slice(0, 300)
    };

    submitOrder(payload);
  });

  function submitOrder(payload) {
    if (!orderCfg.apiEndpoint) {
      var phone = data.store.phone;
      showStatus('error',
        '<i class="fa-solid fa-plug-circle-xmark mt-0.5" aria-hidden="true"></i><span>' +
        '<strong>Đơn hàng chưa được gửi.</strong> Hệ thống đặt hàng trực tuyến đang được kết nối máy chủ. ' +
        (phone
          ? 'Vui lòng gọi <a class="font-semibold underline" href="tel:' + esc(phone) + '">' + esc(phone) + '</a> để đặt sách.'
          : 'Vui lòng thử lại sau hoặc liên hệ nhà sách để đặt hàng.') +
        '</span>');
      return;
    }

    setLoading(true);
    var controller = 'AbortController' in window ? new AbortController() : null;
    var timer = controller ? setTimeout(function () { controller.abort(); }, orderCfg.timeoutMs || 15000) : null;

    var tokenEl = form.querySelector('input[name="__RequestVerificationToken"]');
    var token = tokenEl ? tokenEl.value : '';

    var headers = {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      'Idempotency-Key': idempotencyKey
    };
    if (token) {
      headers['RequestVerificationToken'] = token;
      headers['X-CSRF-TOKEN'] = token;
    }

    fetch(orderCfg.apiEndpoint, {
      method: 'POST',
      headers: headers,
      body: JSON.stringify(payload),
      signal: controller ? controller.signal : undefined
    })
      .then(function (res) {
        return res.json().catch(function () { return {}; }).then(function (body) {
          if (!res.ok) {
            var errMsg = body.message || body.thongBao || (body.eror ? String(body.eror) : null) || 'HTTP ' + res.status;
            var err = new Error(errMsg);
            err.userMessage = errMsg;
            throw err;
          }
          var orderId = body.maDonHang || body.orderId;
          if (!orderId && body.tatus !== 200 && !body.thanhCong) throw new Error('Phản hồi không có mã đơn');
          return body;
        });
      })
      .then(function (order) {
        idempotencyKey = newKey();
        
        // 1. Lấy thông báo từ Server thông qua Response
        var serverMsg = order.message || order.thongBao || 'Đặt hàng thành công!';

        // 2. Kích hoạt Alert (CSS) thông báo trong 5 giây
        showCssAlert('success', serverMsg, 5000);

        // 3. Hiển thị kết quả chi tiết
        showResult(order, payload, serverMsg);
      })
      .catch(function (err) {
        var msg = err.name === 'AbortError'
          ? 'Máy chủ phản hồi quá lâu. Đơn hàng có thể chưa được tạo — vui lòng thử lại.'
          : err.userMessage || 'Không thể gửi đơn hàng do lỗi kết nối hoặc máy chủ. Thông tin của bạn vẫn được giữ, vui lòng thử lại.';
        showStatus('error', '<i class="fa-solid fa-circle-exclamation mt-0.5" aria-hidden="true"></i><span>' + esc(msg) + '</span>');
        showCssAlert('error', msg, 5000);
      })
      .then(function () {
        if (timer) clearTimeout(timer);
        setLoading(false);
      });
  }

  /* ---------- Kết quả (chỉ khi server xác nhận) ---------- */
  var ORDER_STATUS = {
    pending_confirmation: 'Chờ xác nhận',
    confirmed: 'Đã xác nhận',
    shipping: 'Đang giao hàng'
  };
  var PAYMENT_STATUS = {
    unpaid: 'Chưa thanh toán',
    pending: 'Chờ đối soát',
    paid: 'Đã thanh toán'
  };

  function showResult(order, payload, serverMsg) {
    var isBank = (payload.maHinhThucThanhToan || payload.paymentMethod) === 'bank_transfer';
    var pay = order.thongTinThanhToan || order.payment || {};
    var orderId = order.maDonHang || order.orderId;
    var totalAmount = order.tongTien != null ? order.tongTien : order.total;
    var msg = serverMsg || order.message || order.thongBao || 'Đặt hàng thành công!';
    var bankBlock = '';

    var qrUrl = pay.linkQrCode || pay.qrImageUrl;
    var bankName = pay.tenNganHang || pay.bankName;
    var accNum = pay.soTaiKhoan || pay.accountNumber;
    var accName = pay.tenChuTaiKhoan || pay.accountName;
    var content = pay.noiDungChuyenKhoan || pay.transferContent || orderId;

    if (isBank) {
      bankBlock = (accNum || qrUrl)
        ? '<div class="mt-6 rounded-2xl bg-white p-5 text-left shadow-sm">' +
          '  <p class="font-semibold text-royal"><i class="fa-solid fa-building-columns mr-2 text-antique-gold" aria-hidden="true"></i>Thông tin chuyển khoản</p>' +
          (qrUrl ? '<img src="' + esc(qrUrl) + '" alt="Mã QR chuyển khoản cho đơn ' + esc(orderId) + '" class="mx-auto my-4 w-48 rounded-lg" width="192" height="192">' : '') +
          '  <dl class="mt-3 grid grid-cols-[auto_1fr] gap-x-4 gap-y-1 text-sm">' +
          (bankName ? '<dt class="text-ink/60">Ngân hàng</dt><dd class="font-medium">' + esc(bankName) + '</dd>' : '') +
          (accNum ? '<dt class="text-ink/60">Số tài khoản</dt><dd class="font-medium">' + esc(accNum) + '</dd>' : '') +
          (accName ? '<dt class="text-ink/60">Chủ tài khoản</dt><dd class="font-medium">' + esc(accName) + '</dd>' : '') +
          '    <dt class="text-ink/60">Nội dung</dt><dd class="font-semibold text-royal">' + esc(content) + '</dd>' +
          '  </dl>' +
          '  <p class="mt-3 text-xs text-ink/60">Đơn sẽ được cập nhật “Đã thanh toán” sau khi nhà sách đối soát giao dịch.</p>' +
          '</div>'
        : '<p class="mt-6 rounded-xl bg-white p-4 text-sm text-ink/80">Nhà sách sẽ liên hệ để gửi thông tin chuyển khoản cho đơn <strong>' + esc(orderId) + '</strong>.</p>';
    }

    result.innerHTML =
      '<div class="py-4 text-center">' +
      '  <div class="alert alert-success text-left mb-6" role="alert">' +
      '    <i class="fa-solid fa-circle-check text-2xl shrink-0" aria-hidden="true"></i>' +
      '    <div class="flex-1">' +
      '      <strong class="font-bold block text-base">' + esc(msg) + '</strong>' +
      '      <span class="text-xs text-ink/80">Mã đơn hàng <strong>' + esc(orderId) + '</strong> đã được ghi nhận trên hệ thống.</span>' +
      '    </div>' +
      '  </div>' +
      '  <div class="result-icon"><i class="fa-solid fa-check" aria-hidden="true"></i></div>' +
      '  <h3 class="mt-6 font-display text-2xl text-royal">Đã ghi nhận đơn hàng</h3>' +
      '  <p class="mt-2 text-sm text-ink/75">Mã đơn của bạn: <strong class="text-royal">' + esc(orderId) + '</strong></p>' +
      '  <div class="mt-4 flex flex-wrap justify-center gap-2">' +
      '    <span class="status-pill status-pill--pending"><i class="fa-regular fa-clock" aria-hidden="true"></i>' + esc(order.tenTinhTrang || ORDER_STATUS[order.maTinhTrang] || ORDER_STATUS[order.status] || 'Chờ xác nhận') + '</span>' +
      '    <span class="status-pill status-pill--unpaid"><i class="fa-solid fa-wallet" aria-hidden="true"></i>' + esc(PAYMENT_STATUS[order.paymentStatus] || 'Chưa thanh toán') + '</span>' +
      '  </div>' +
      (typeof totalAmount === 'number' && totalAmount > 0
        ? '<p class="mt-4 text-sm">Tổng thanh toán: <strong class="text-lg text-royal">' + fmt(totalAmount) + '</strong></p>' : '') +
      bankBlock +
      '  <p class="mt-6 text-sm text-ink/70">' + (isBank
        ? 'Vui lòng chuyển khoản đúng nội dung để đơn được xử lý nhanh nhất.'
        : 'Nhà sách sẽ gọi điện xác nhận trước khi giao hàng.') + '</p>' +
      '  <button type="button" class="btn btn-royal mt-6" data-close-order>Hoàn tất</button>' +
      '</div>';

    result.querySelector('[data-close-order]').addEventListener('click', function () { dialog.close(); });
    form.hidden = true;
    result.hidden = false;
    result.focus();
  }

  function resetToForm(clear) {
    if (clear) {
      form.reset();
      triedSubmit = false;
      Object.keys(validators).forEach(function (name) {
        var field = form.elements[name];
        if (field) field.removeAttribute('aria-invalid');
        var err = document.getElementById('e-' + name);
        if (err) { err.innerHTML = ''; err.classList.remove('is-visible'); }
      });
      showStatus('', '');
      updateSummary();
    }
    result.hidden = true;
    result.innerHTML = '';
    form.hidden = false;
  }

  updateSummary();

  /* ---------- Sticky CTA mobile ---------- */
  var sticky = document.getElementById('sticky-cta');
  var heroCta = document.getElementById('hero-cta');
  if (sticky && heroCta && 'IntersectionObserver' in window) {
    new IntersectionObserver(function (entries) {
      var entry = entries[0];
      var show = !entry.isIntersecting && entry.boundingClientRect.top < 0;
      sticky.classList.toggle('is-visible', show);
      sticky.setAttribute('aria-hidden', show ? 'false' : 'true');
      if (show) sticky.removeAttribute('inert');
      else sticky.setAttribute('inert', '');
    }).observe(heroCta);
  }
})();
