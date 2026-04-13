export type DisplayColumnKey = 'Year' | 'Discipline' | 'Strength' | 'ExecutionModel' | 'MemoryManagement' | 'Tags';

export interface LanguageModelMeta {
  year: string;
  discipline: string;
  strength: string;
  executionModel: string;
  memoryManagement: string;
  tags: string;
}

export const LANGDLE_DISPLAY_COLUMNS: Array<{ key: DisplayColumnKey; label: string }> = [
  { key: 'Year', label: 'Year' },
  { key: 'Discipline', label: 'Discipline' },
  { key: 'Strength', label: 'Strength' },
  { key: 'ExecutionModel', label: 'Execution' },
  { key: 'MemoryManagement', label: 'Memory' },
  { key: 'Tags', label: 'Tags' }
];

export const LANGDLE_LANGUAGE_META: Record<string, LanguageModelMeta> = {
  python: { year: '1991', discipline: 'Dynamic', strength: 'Strong', executionModel: 'Interpreted', memoryManagement: 'GarbageCollected', tags: 'Scripting, Data Science, Machine Learning, Automation' },
  javascript: { year: '1995', discipline: 'Dynamic', strength: 'Weak', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Web, Scripting, Browser' },
  java: { year: '1995', discipline: 'Static', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Enterprise, Mobile, JVM' },
  c: { year: '1972', discipline: 'Static', strength: 'Weak', executionModel: 'Compiled', memoryManagement: 'Manual', tags: 'Systems, Embedded, Operating Systems' },
  'c++': { year: '1985', discipline: 'Static', strength: 'Strong', executionModel: 'Compiled', memoryManagement: 'Manual', tags: 'Systems, Game Development, Low-Level' },
  'c#': { year: '2000', discipline: 'Static', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Enterprise, Game Development, .NET' },
  typescript: { year: '2012', discipline: 'Static', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Web, Browser' },
  ruby: { year: '1995', discipline: 'Dynamic', strength: 'Strong', executionModel: 'Interpreted', memoryManagement: 'GarbageCollected', tags: 'Web, Scripting, DevOps' },
  php: { year: '1995', discipline: 'Dynamic', strength: 'Weak', executionModel: 'Interpreted', memoryManagement: 'GarbageCollected', tags: 'Web, Scripting' },
  swift: { year: '2014', discipline: 'Static', strength: 'Strong', executionModel: 'Compiled', memoryManagement: 'ARC', tags: 'Mobile, Desktop' },
  kotlin: { year: '2011', discipline: 'Static', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Mobile, Enterprise, JVM' },
  go: { year: '2009', discipline: 'Static', strength: 'Strong', executionModel: 'Compiled', memoryManagement: 'GarbageCollected', tags: 'Systems, Cloud, DevOps' },
  rust: { year: '2010', discipline: 'Static', strength: 'Strong', executionModel: 'Compiled', memoryManagement: 'OwnershipBorrowing', tags: 'Systems, Embedded, Low-Level' },
  r: { year: '1993', discipline: 'Dynamic', strength: 'Strong', executionModel: 'Interpreted', memoryManagement: 'GarbageCollected', tags: 'Data Science, Scientific Computing' },
  matlab: { year: '1984', discipline: 'Dynamic', strength: 'Strong', executionModel: 'Interpreted', memoryManagement: 'GarbageCollected', tags: 'Scientific Computing, Engineering' },
  scala: { year: '2004', discipline: 'Static', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Enterprise, Data Science, JVM' },
  perl: { year: '1987', discipline: 'Dynamic', strength: 'Weak', executionModel: 'Interpreted', memoryManagement: 'GarbageCollected', tags: 'Scripting, Text Processing, DevOps' },
  lua: { year: '1993', discipline: 'Dynamic', strength: 'Weak', executionModel: 'Interpreted', memoryManagement: 'GarbageCollected', tags: 'Game Development, Embedded, Scripting' },
  haskell: { year: '1990', discipline: 'Static', strength: 'Strong', executionModel: 'Compiled', memoryManagement: 'GarbageCollected', tags: 'Academic, Compiler Design' },
  dart: { year: '2011', discipline: 'Static', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Mobile, Web' },
  'objective-c': { year: '1984', discipline: 'Static', strength: 'Weak', executionModel: 'Compiled', memoryManagement: 'ARC', tags: 'Mobile, Desktop' },
  assembly: { year: '1949', discipline: 'Static', strength: 'Weak', executionModel: 'Compiled', memoryManagement: 'Manual', tags: 'Systems, Embedded, Low-Level' },
  elixir: { year: '2011', discipline: 'Dynamic', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Web, Distributed, BEAM' },
  clojure: { year: '2007', discipline: 'Dynamic', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Enterprise, Data Science, JVM' },
  'f#': { year: '2005', discipline: 'Static', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Enterprise, Scientific Computing, .NET' },
  erlang: { year: '1986', discipline: 'Dynamic', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Telecom, Distributed, BEAM' },
  julia: { year: '2012', discipline: 'Dynamic', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Scientific Computing, Data Science' },
  groovy: { year: '2003', discipline: 'Dynamic', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Scripting, Enterprise, JVM' },
  'visual basic': { year: '1991', discipline: 'Static', strength: 'Strong', executionModel: 'BytecodeJIT', memoryManagement: 'GarbageCollected', tags: 'Enterprise, Desktop, .NET' },
  powershell: { year: '2006', discipline: 'Dynamic', strength: 'Strong', executionModel: 'Interpreted', memoryManagement: 'GarbageCollected', tags: 'Scripting, Automation, DevOps' },
  pascal: { year: '1970', discipline: 'Static', strength: 'Strong', executionModel: 'Compiled', memoryManagement: 'Manual', tags: 'Academic, Desktop' },
  prolog: { year: '1972', discipline: 'Dynamic', strength: 'Strong', executionModel: 'Interpreted', memoryManagement: 'GarbageCollected', tags: 'Academic, AI' }
};

export const LANGDLE_LANGUAGE_OPTIONS: string[] = [
  'Python',
  'JavaScript',
  'Java',
  'C',
  'C++',
  'C#',
  'TypeScript',
  'Ruby',
  'PHP',
  'Swift',
  'Kotlin',
  'Go',
  'Rust',
  'R',
  'MATLAB',
  'Scala',
  'Perl',
  'Lua',
  'Haskell',
  'Dart',
  'Objective-C',
  'Assembly',
  'Elixir',
  'Clojure',
  'F#',
  'Erlang',
  'Julia',
  'Groovy',
  'Visual Basic',
  'PowerShell',
  'Pascal',
  'Prolog'
];
