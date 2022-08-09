using System;
using WeSafe.Shared.Abstract;

namespace WeSafe.Services.Client.Models
{
    public class CameraMetadataModel : MetadataModel, IHasId
    {
        public int Id { get; set; }
    }
}