import { useEffect, useRef } from 'react';

export function useAnimationFrame(onFrame: (elapsedMilliseconds: number) => void): void {
  const callback = useRef(onFrame);
  callback.current = onFrame;

  useEffect(() => {
    let previous = performance.now();
    let handle = requestAnimationFrame(function tick(now) {
      callback.current(now - previous);
      previous = now;
      handle = requestAnimationFrame(tick);
    });
    return () => cancelAnimationFrame(handle);
  }, []);
}
