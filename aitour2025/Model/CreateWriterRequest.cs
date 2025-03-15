// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Model;

public class CreateWriterRequest
{
    public string Research { get; set; }
    public string Writing { get; set; }

    public SupportRequest supportRequest { get; set; }
}
