using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace kentaasvang.ServerWorker;

// TODO: create validation for this class
public class WorkerSettings
{
    public List<Service>? Services { get; set; }
    public int DelayInMilliSeconds { get; set; }
    public string StartVersion { get; set; }
}