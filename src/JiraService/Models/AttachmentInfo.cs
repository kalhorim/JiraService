﻿using System;
using System.Collections.Generic;
using System.Text;

namespace JiraService.Models
{
    public class AttachmentInfo : CommentModel
    {
        public string FileName { get; set; }
        public byte[] DataBytes { get; set; }
    }
}
