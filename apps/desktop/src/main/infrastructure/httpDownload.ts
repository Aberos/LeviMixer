export type DownloadProgress = (fraction: number | null) => void;

const attemptTimeoutMilliseconds = 3 * 60 * 1_000;
const maximumAttempts = 5;
const partialContent = 206;

interface PartialDownload {
  readonly chunks: Uint8Array[];
  received: number;
  total: number | null;
}

export async function httpDownload(url: string, onProgress: DownloadProgress): Promise<Uint8Array | null> {
  const partial: PartialDownload = { chunks: [], received: 0, total: null };
  for (let attempt = 0; attempt < maximumAttempts; attempt++) {
    if (await resume(url, partial, onProgress)) {
      return joined(partial.chunks, partial.received);
    }
  }

  return null;
}

async function resume(url: string, partial: PartialDownload, onProgress: DownloadProgress): Promise<boolean> {
  try {
    const headers: Record<string, string> = partial.received > 0 ? { Range: `bytes=${partial.received}-` } : {};
    const response = await fetch(url, { headers, signal: AbortSignal.timeout(attemptTimeoutMilliseconds) });
    if (!response.ok || response.body === null) {
      return false;
    }

    if (partial.received > 0 && response.status !== partialContent) {
      partial.chunks.length = 0;
      partial.received = 0;
    }

    partial.total ??= Number(response.headers.get('content-length')) || null;
    for await (const chunk of response.body) {
      partial.chunks.push(chunk);
      partial.received += chunk.byteLength;
      onProgress(partial.total === null ? null : partial.received / partial.total);
    }

    return partial.total === null || partial.received >= partial.total;
  } catch {
    return false;
  }
}

function joined(chunks: readonly Uint8Array[], length: number): Uint8Array {
  const result = new Uint8Array(length);
  let offset = 0;
  for (const chunk of chunks) {
    result.set(chunk, offset);
    offset += chunk.byteLength;
  }

  return result;
}
