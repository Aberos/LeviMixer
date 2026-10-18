export interface EngineTransport {
  send(line: string): void;
  onLine(listener: (line: string) => void): void;
  onExit(listener: (reason: string) => void): void;
  kill(): void;
}
