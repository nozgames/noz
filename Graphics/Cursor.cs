using System;
using System.Collections.Generic;
using System.Text;

namespace NoZ {

    [SerializedType]
    [SharedResource]
    public class Cursor : IResource {
        Resource IResource.Resource { get; set; }
    }
}
