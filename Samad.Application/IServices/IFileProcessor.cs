using Samad.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Samad.Application.IServices
{
    public interface IFileProcessor
    {
        bool CanProcess(DetectedFile file);

        Task<ProcessedFile> ProcessAsync(
            IncomingFile input,
            DetectedFile detected,
            long maxOutputBytes,
            CancellationToken cancellationToken = default);
    }
}
