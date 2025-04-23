using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SoftableDrive.TempScaffold;

[PrimaryKey("Id", "Uploadtime", "Name", "Size")]
[Table("files")]
public partial class File
{
    [Key]
    public string Id { get; set; } = null!;

    [Key]
    public string Uploadtime { get; set; } = null!;

    [Key]
    public string Name { get; set; } = null!;

    [Key]
    public int Size { get; set; }
}
