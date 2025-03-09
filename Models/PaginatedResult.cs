using System.Collections.Generic;

namespace RepairSystem.API.Models
{
    /// <summary>
    /// 分頁結果泛型類，用於封裝分頁數據和分頁信息
    /// </summary>
    /// <typeparam name="T">分頁數據項的類型</typeparam>
    /// <example>
    /// {
    ///   "items": [
    ///     { "id": 1, "name": "Item 1" },
    ///     { "id": 2, "name": "Item 2" }
    ///   ],
    ///   "totalItems": 50,
    ///   "page": 1,
    ///   "pageSize": 10,
    ///   "totalPages": 5,
    ///   "hasPrevious": false,
    ///   "hasNext": true
    /// }
    /// </example>
    public class PaginatedResult<T>
    {
        /// <summary>
        /// 當前頁的數據項集合
        /// </summary>
        public IEnumerable<T> Items { get; set; } = new List<T>();
        
        /// <summary>
        /// 符合條件的總數據項數量
        /// </summary>
        /// <example>50</example>
        public int TotalItems { get; set; }
        
        /// <summary>
        /// 當前頁碼，從1開始
        /// </summary>
        /// <example>1</example>
        public int Page { get; set; }
        
        /// <summary>
        /// 每頁數據項數量
        /// </summary>
        /// <example>10</example>
        public int PageSize { get; set; }
        
        /// <summary>
        /// 總頁數
        /// </summary>
        /// <example>5</example>
        public int TotalPages => (TotalItems + PageSize - 1) / PageSize;
        
        /// <summary>
        /// 是否有前一頁
        /// </summary>
        /// <example>false</example>
        public bool HasPrevious => Page > 1;
        
        /// <summary>
        /// 是否有下一頁
        /// </summary>
        /// <example>true</example>
        public bool HasNext => Page < TotalPages;
    }
} 