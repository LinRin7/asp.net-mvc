using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class Joke
    {
        //第一次執行程式時
        //需新增NuGet套件。工具->NuGet套件管理員->套件管理器主控台
        //執行以下
        //Install-Package Microsoft.EntityFrameworkCore.Design
        //Install-Package Microsoft.EntityFrameworkCore.SqlServer
        //此為Microsoft SQL Service，若是其他資料庫則需改為該資料庫的指令
        //SQL Server EF Core 提供者。 提供者套件會將 EF Core 套件安裝為相依性。

        //使用EF Core 移轉功能來建立資料庫。 移轉 是一組工具，可建立和更新資料庫以符合資料模型。
        //工具->NuGet 套件管理員套件管理員->主控台
        //Add-Migration InitialCreate//產生移轉檔案
        //Update-Database//更新資料庫到先前命令所建立的最新移轉。

        //若有資料表需要新增參數，則需再執行Add-Migration '檔案名稱' 來產生新的移轉檔案
        //再執行Update-Database

        //若有新增的類別，要新增資料表，需先更新 '專案名稱'.Data 裡的DbContext.cs檔案，否則不會新增成資料表


        public int Id { get; set; }

        [Required]
        [Display(Name = "問題")]
        public string JokeQuestion { get; set; }

        [Required]
        [Display(Name = "答案")]
        public string JokeAnswer { get; set; }

        [BindNever]
        [Display(Name = "作者")]
        public string Owner { get; set; }
        public Joke()
        {

        }
        public Joke(int id, string jokeQuestion, string jokeAnswer, string owner)
        {
            Id = id;
            JokeQuestion = jokeQuestion;
            JokeAnswer = jokeAnswer;
            Owner = owner;
        }
    }
}
