import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StockDialog } from './stock-dialog';

describe('StockDialog', () => {
  let component: StockDialog;
  let fixture: ComponentFixture<StockDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StockDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StockDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
