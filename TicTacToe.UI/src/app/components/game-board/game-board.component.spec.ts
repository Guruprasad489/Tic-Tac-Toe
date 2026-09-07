import { ComponentFixture, TestBed } from '@angular/core/testing';
import { GameBoardComponent } from './game-board.component';
import { GameStatus, GameMode, Player } from '../../models/game.models';

describe('GameBoardComponent', () => {
  let component: GameBoardComponent;
  let fixture: ComponentFixture<GameBoardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GameBoardComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(GameBoardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display X in a cell when board has X', () => {
    component.game = {
      gameId: 'test-id',
      board: [Player.X, null, null, null, null, null, null, null, null],
      currentPlayer: Player.O,
      gameMode: GameMode.TwoPlayer,
      gameStatus: GameStatus.InProgress,
      winner: null,
      winningCells: [],
      moveHistory: [],
      scoreboard: { xWins: 0, oWins: 0, draws: 0 }
    };
    fixture.detectChanges();

    const cells = fixture.nativeElement.querySelectorAll('.cell');
    expect(cells[0].textContent).toContain('X');
  });

  it('should emit cellClicked when a cell is clicked', (done) => {
    component.game = {
      gameId: 'test-id',
      board: Array(9).fill(null),
      currentPlayer: Player.X,
      gameMode: GameMode.TwoPlayer,
      gameStatus: GameStatus.InProgress,
      winner: null,
      winningCells: [],
      moveHistory: [],
      scoreboard: { xWins: 0, oWins: 0, draws: 0 }
    };
    fixture.detectChanges();

    component.cellClicked.subscribe((index: number) => {
      expect(index).toBe(0);
      //done();
    });

    const cells = fixture.nativeElement.querySelectorAll('.cell');
    cells[0].click();
  });

  it('should disable cells when game is completed (Won)', () => {
    component.game = {
      gameId: 'test-id',
      board: Array(9).fill(null),
      currentPlayer: Player.X,
      gameMode: GameMode.TwoPlayer,
      gameStatus: GameStatus.Won,
      winner: Player.X,
      winningCells: [0, 1, 2],
      moveHistory: [],
      scoreboard: { xWins: 0, oWins: 0, draws: 0 }
    };
    fixture.detectChanges();

    const cells = fixture.nativeElement.querySelectorAll('.cell');
    cells.forEach((cell: HTMLElement) => {
      expect(cell.getAttribute('disabled')).not.toBeNull();
    });
  });

  it('should highlight winning cells', () => {
    component.game = {
      gameId: 'test-id',
      board: [Player.X, Player.X, Player.X, null, null, null, null, null, null],
      currentPlayer: Player.O,
      gameMode: GameMode.TwoPlayer,
      gameStatus: GameStatus.Won,
      winner: Player.X,
      winningCells: [0, 1, 2],
      moveHistory: [],
      scoreboard: { xWins: 0, oWins: 0, draws: 0 }
    };
    fixture.detectChanges();

    const cells = fixture.nativeElement.querySelectorAll('.cell.winning');
    expect(cells.length).toBe(3);
  });

  it('should display winner message when game is won', () => {
    component.game = {
      gameId: 'test-id',
      board: Array(9).fill(null),
      currentPlayer: Player.O,
      gameMode: GameMode.TwoPlayer,
      gameStatus: GameStatus.Won,
      winner: Player.X,
      winningCells: [],
      moveHistory: [],
      scoreboard: { xWins: 0, oWins: 0, draws: 0 }
    };
    fixture.detectChanges();

    const status = fixture.nativeElement.querySelector('.winner');
    expect(status.textContent).toContain('Player X Wins');
  });

  it('should display draw message when game is a draw', () => {
    component.game = {
      gameId: 'test-id',
      board: Array(9).fill(Player.X),
      currentPlayer: Player.O,
      gameMode: GameMode.TwoPlayer,
      gameStatus: GameStatus.Draw,
      winner: null,
      winningCells: [],
      moveHistory: [],
      scoreboard: { xWins: 0, oWins: 0, draws: 0 }
    };
    fixture.detectChanges();

    const status = fixture.nativeElement.querySelector('.draw');
    expect(status.textContent).toContain("It's a Draw");
  });
});
