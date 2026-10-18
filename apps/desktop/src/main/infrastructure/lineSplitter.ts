export class LineSplitter {
  #buffer = '';

  push(chunk: string): string[] {
    this.#buffer += chunk;
    const lines = this.#buffer.split('\n');
    this.#buffer = lines.pop() ?? '';
    return lines.map((line) => line.trim()).filter((line) => line.length > 0);
  }
}
