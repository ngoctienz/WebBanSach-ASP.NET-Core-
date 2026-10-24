namespace WebBanSach.Models.DTOs
{
    /// <summary>
    /// Response model chung cho toàn bộ hệ thống
    /// </summary>
    public class Responses<T>
    {
        // Các field chính theo đúng yêu cầu: Tatus, message, Eror
        public int Tatus { get; set; } = 200;
        public string? message { get; set; }
        public object? Eror { get; set; }
        public T? Data { get; set; }

        // Alias tương thích viết hoa / đúng chính tả tiếng Anh
        public int Status
        {
            get { return Tatus; }
            set { Tatus = value; }
        }

        public string? Message
        {
            get { return message; }
            set { message = value; }
        }

        public object? Error
        {
            get { return Eror; }
            set { Eror = value; }
        }
    }

    /// <summary>
    /// Response chung không định kiểu Data (object)
    /// </summary>
    public class Responses : Responses<object>
    {
        public Responses()
        {
        }

        public Responses(int tatus, string? msg, object? eror = null, object? data = null)
        {
            Tatus = tatus;
            message = msg;
            Eror = eror;
            Data = data;
        }

        public static Responses Success(string? msg = "Thành công", object? data = null)
        {
            return new Responses
            {
                Tatus = 200,
                message = msg,
                Eror = null,
                Data = data
            };
        }

        public static Responses Fail(string? msg = "Thất bại", object? eror = null, int tatus = 400)
        {
            return new Responses
            {
                Tatus = tatus,
                message = msg,
                Eror = eror,
                Data = null
            };
        }
    }
}
