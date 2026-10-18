import { useState, type KeyboardEvent } from 'react';

const maxLength = 24;

interface ChannelNameProps {
  readonly name: string;
  readonly onRename: (name: string) => void;
}

export function ChannelName({ name, onRename }: ChannelNameProps) {
  const [draft, setDraft] = useState<string | null>(null);

  const commit = () => {
    const trimmed = draft?.trim() ?? '';
    if (trimmed.length > 0 && trimmed !== name) {
      onRename(trimmed);
    }

    setDraft(null);
  };

  const handleKey = (event: KeyboardEvent<HTMLInputElement>) => {
    if (event.key === 'Enter') {
      commit();
    } else if (event.key === 'Escape') {
      setDraft(null);
    }
  };

  if (draft !== null) {
    return (
      <input
        className="strip-name-input"
        aria-label="Nome do canal"
        value={draft}
        maxLength={maxLength}
        autoFocus
        onChange={(event) => setDraft(event.target.value)}
        onBlur={commit}
        onKeyDown={handleKey}
      />
    );
  }

  return (
    <button type="button" className="strip-name" title="Clique duas vezes para renomear" onDoubleClick={() => setDraft(name)}>
      {name}
    </button>
  );
}
