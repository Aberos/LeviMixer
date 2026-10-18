import { mkdtemp, readFile, rm, writeFile } from 'node:fs/promises';
import { tmpdir } from 'node:os';
import { join } from 'node:path';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import { savedSession } from '../testing/fixtures.ts';
import { JsonSessionRepository } from './jsonSessionRepository.ts';

describe('JsonSessionRepository', () => {
  let directory: string;

  beforeEach(async () => {
    directory = await mkdtemp(join(tmpdir(), 'mixer-session-'));
  });

  afterEach(async () => {
    await rm(directory, { recursive: true, force: true });
  });

  it('round-trips a session', async () => {
    const repository = new JsonSessionRepository(join(directory, 'nested', 'session.json'));

    await repository.save(savedSession);

    await expect(repository.load()).resolves.toEqual(savedSession);
  });

  it('loads nothing when the file is missing', async () => {
    await expect(new JsonSessionRepository(join(directory, 'missing.json')).load()).resolves.toBeNull();
  });

  it('loads nothing when the file is corrupt', async () => {
    const path = join(directory, 'session.json');
    await writeFile(path, '{ broken', 'utf8');

    await expect(new JsonSessionRepository(path).load()).resolves.toBeNull();
  });

  it('writes human readable JSON', async () => {
    const path = join(directory, 'session.json');

    await new JsonSessionRepository(path).save(savedSession);

    expect(await readFile(path, 'utf8')).toContain('\n  "schemaVersion": 1');
  });
});
