export interface ClusterdleTile {
  id: string;
  label: string;
}

export interface ClusterdleBoard {
  tiles: ClusterdleTile[];
}

export const CLUSTERDLE_DUMMY_BOARD: ClusterdleBoard = {
  tiles: [
    { id: '1', label: 'Angular' },
    { id: '2', label: 'Docker' },
    { id: '3', label: 'Tailwind' },
    { id: '4', label: 'Redis' },
    { id: '5', label: 'RabbitMQ' },
    { id: '6', label: 'Postgres' },
    { id: '7', label: 'Nginx' },
    { id: '8', label: 'TypeScript' }
  ]
};
