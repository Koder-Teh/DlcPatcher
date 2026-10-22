using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DlcPatcher
{
    [JsonSerializable(typeof(Sample))]
    internal partial class SourceGenerationContext : JsonSerializerContext { }

    internal class Sample
    {
        [JsonPropertyName("patchs")]
        public List<Patchs> patchs { get; set; }
    }

    internal class Patchs
    {
        [JsonPropertyName("file_offset")]
        public string offset { get; set; }

        [JsonPropertyName("restore_byte")]
        public string restore { get; set; }

        [JsonPropertyName("patch_byte")]
        public string patch { get; set; }
    }
}
