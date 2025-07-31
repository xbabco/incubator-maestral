// Copyright © 2025 xbabco. All rights reserved.

namespace Maestral.Core.Services;

public interface ITextRecognizer
{
    Task<RecognizedTextResult> RecognizeTextAsync(Stream imageStream);
}
