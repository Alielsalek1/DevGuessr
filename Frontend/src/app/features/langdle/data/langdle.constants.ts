export type DisplayColumnKey = 'Year' | 'TypeChecking' | 'Memory' | 'ScopeSyntax' | 'Semicolons' | 'Tags';

export const LANGDLE_DISPLAY_COLUMNS: Array<{ key: DisplayColumnKey; label: string }> = [
  { key: 'Year', label: 'Year' },
  { key: 'TypeChecking', label: 'Type Checking' },
  { key: 'Memory', label: 'Memory' },
  { key: 'ScopeSyntax', label: 'Scope Syntax' },
  { key: 'Semicolons', label: 'Semicolons' },
  { key: 'Tags', label: 'Tags' }
];

export const LANGDLE_LANGUAGE_OPTIONS: string[] = [
  'Python',
  'JavaScript',
  'Java',
  'C++',
  'C',
  'Rust',
  'Go',
  'Swift',
  'Kotlin',
  'TypeScript',
  'PHP',
  'Ruby',
  'C#',
  'Haskell',
  'Scala',
  'Erlang',
  'Elixir',
  'Lua',
  'Perl',
  'R',
  'Julia',
  'Dart',
  'Zig',
  'F#',
  'Objective-C',
  'Pascal',
  'COBOL',
  'Ada',
  'MATLAB',
  'PowerShell',
];
