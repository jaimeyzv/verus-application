import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { Company } from '../../models/company.model';
import { CompanyList } from './company-list';

describe('CompanyList', () => {
  let component: CompanyList;
  let fixture: ComponentFixture<CompanyList>;

  const sample: Company = {
    id: '1',
    name: 'Acme Widgets',
    website: 'https://acmewidgets.com/',
    websiteDomain: 'acmewidgets.com',
    createdAtUtc: '2026-01-01T00:00:00Z',
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CompanyList],
      providers: [provideRouter([])],
    }).compileComponents();
    fixture = TestBed.createComponent(CompanyList);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    fixture.componentRef.setInput('companies', []);
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('renders the provided empty message when there are no companies', () => {
    fixture.componentRef.setInput('companies', []);
    fixture.componentRef.setInput('emptyMessage', 'Nothing here');
    fixture.detectChanges();
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('Nothing here');
  });

  it('renders a row per company', () => {
    fixture.componentRef.setInput('companies', [sample]);
    fixture.detectChanges();
    const rows = (fixture.nativeElement as HTMLElement).querySelectorAll('tbody tr');
    expect(rows.length).toBe(1);
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('Acme Widgets');
  });
});
