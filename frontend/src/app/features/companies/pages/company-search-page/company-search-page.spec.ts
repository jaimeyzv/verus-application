import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { errorInterceptor } from '../../../../core/errors/error-interceptor';
import { Company } from '../../models/company.model';
import { CompanySearchPage } from './company-search-page';

describe('CompanySearchPage', () => {
  let component: CompanySearchPage;
  let fixture: ComponentFixture<CompanySearchPage>;
  let httpMock: HttpTestingController;

  const sample: Company = {
    id: '1',
    name: 'Globex',
    website: 'https://globex.com/',
    websiteDomain: 'globex.com',
    createdAtUtc: '2026-01-01T00:00:00Z',
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CompanySearchPage],
      providers: [
        provideHttpClient(withInterceptors([errorInterceptor])),
        provideHttpClientTesting(),
        provideRouter([]),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CompanySearchPage);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
  });

  afterEach(() => httpMock.verify());

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('sends only the populated criteria and stores the results', () => {
    component.form.setValue({ name: '', domain: '', term: 'globex' });

    component.search();

    const req = httpMock.expectOne((r) => r.url.endsWith('/api/companies/search'));
    expect(req.request.params.get('term')).toBe('globex');
    expect(req.request.params.has('name')).toBeFalse();
    req.flush([sample]);

    expect(component.searched()).toBeTrue();
    expect(component.results().length).toBe(1);
  });

  it('marks the search complete with an empty result set', () => {
    component.form.setValue({ name: 'nothing', domain: '', term: '' });

    component.search();

    httpMock.expectOne((r) => r.url.endsWith('/api/companies/search')).flush([]);

    expect(component.searched()).toBeTrue();
    expect(component.results()).toEqual([]);
  });
});
