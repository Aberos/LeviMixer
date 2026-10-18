import '@testing-library/jest-dom/vitest';
import { cleanup } from '@testing-library/react';
import { afterEach } from 'vitest';

const capturedPointers = new WeakMap<Element, Set<number>>();

function pointersOf(element: Element): Set<number> {
  const pointers = capturedPointers.get(element) ?? new Set<number>();
  capturedPointers.set(element, pointers);
  return pointers;
}

if (typeof Element !== 'undefined' && !('setPointerCapture' in Element.prototype)) {
  Object.assign(Element.prototype, {
    setPointerCapture(this: Element, pointerId: number) {
      pointersOf(this).add(pointerId);
    },
    releasePointerCapture(this: Element, pointerId: number) {
      pointersOf(this).delete(pointerId);
    },
    hasPointerCapture(this: Element, pointerId: number) {
      return pointersOf(this).has(pointerId);
    },
  });
}

afterEach(() => {
  cleanup();
});
