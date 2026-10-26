//DTO
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using EmployeeModel;

//POST用
public class CreateEmployeeDto
{
    //[Required]
    //[StringLength(5)]
    //public string ShainNo { get; set; } = ""; //社員番号

    [Required]
    [StringLength(20)]
    public string Name { get; set; } = ""; //社員名

    [Required]
    [StringLength(20)]
    public string Busho { get; set; } = ""; //部署

    [Range(0, 100)]
    public Int32 Age { get; set; } //年齢

    [StringLength(50)]
    public string? Hobby { get; set; } //趣味

    public bool IsDeleted { get; set; } = false; //論理削除フラグ
}

//PUT用
public class UpdateEmployeeDto
{
    [Required]
    [StringLength(5)]
    public string ShainNo { get; set; } = ""; //社員番号

    [Required]
    [StringLength(20)]
    public string Name { get; set; } = ""; //社員名

    [Required]
    [StringLength(20)]
    public string Busho { get; set; } = ""; //部署

    [Range(0, 100)]
    public int Age { get; set; } //年齢

    [StringLength(50)]
    public string? Hobby { get; set; } //趣味

    public bool IsDeleted { get; set; } = false; //論理削除フラグ

}



//練習用ーーーーーーーーーーーーーーーーー

public class SearchResponceDto
{
    [Required]
    [StringLength(5)]
    public string ShainNo { get; set; } = ""; //社員番号

    [Required]
    [StringLength(20)]
    public string Name { get; set; } = ""; //社員名

    [Required]
    [StringLength(20)]
    public string Busho { get; set; } = ""; //部署

    [Range(0, 100)]
    public int Age { get; set; } //年齢
}

public class SearchResultDto
{
    public SearchResult result { get; set; }

    public List<SearchResponceDto>? employees { get; set; }
}


//練習用ーーーーーーーーーーーーーーーーー
