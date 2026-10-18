import { describe, expect, it } from 'vitest';
import { LineSplitter } from './lineSplitter.ts';

describe('LineSplitter', () => {
  it('returns complete lines and keeps the partial tail', () => {
    const splitter = new LineSplitter();

    expect(splitter.push('{"a":1}\n{"b"')).toEqual(['{"a":1}']);
    expect(splitter.push(':2}\n\n')).toEqual(['{"b":2}']);
  });
});
