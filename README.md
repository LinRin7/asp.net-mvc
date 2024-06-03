Asp.Net Core WebApp (MVC) .NET 6.0<br /><br />

資料庫：MSSQL 存取：Entity Framework, LINQ<br />
驗證：Cookie身分驗證(驗證方案：CookieAuthenticationDefaults.AuthenticationScheme)<br /><br />

功能：<br /><br />

1.會員：<br />
	訪客：登入、註冊會員。<br />
	會員：資料修改。<br /><br />


2.笑話問答分享：(參考YT影片：ASP.NET Core Crash Course - C# App in One Hour)<br />
	訪客：瀏覽笑話。<br />
	會員：創建、編輯自己建立的笑話。<br /><br />


3.商品：<br />
	訪客：瀏覽商品。<br />
	會員：將商品加至願望清單。<br /><br />


4.商品的Restful Api：<br />
	使用postman對商品的CRUD<br />
	4.1.取得全部商品(Get)<br />
	4.2.取得指定Id商品(Get)<br />
	4.3.新增一個商品(post)(有設定驗證身分)<br />
	4.4.更新指定Id商品(put)(有設定驗證身分)<br />
	4.5.更新指定Id商品(patch)(有設定驗證身分)<br />
	4.6.刪除指定Id商品(delete)(有設定驗證身分)<br /><br />


5.GoogleMap(Maps Javascript Api)<br />
	使用Google Cloud提供的Api<br />
	5.1可在Google Map上搜尋自身所在位置。<br />
	5.2可點擊位置並搜尋當前位置的餐廳<br />
