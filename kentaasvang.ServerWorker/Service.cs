using System.ComponentModel.DataAnnotations;

namespace kentaasvang.ServerWorker;

// TODO: create validation for this class
public class Service
{
    public string? Name { get; set; }
    public string? Published { get; set; }
    public string? Versions { get; set; }
    public string? Current { get; set; }
}