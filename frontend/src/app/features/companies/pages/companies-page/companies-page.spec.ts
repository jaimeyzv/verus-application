import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { errorInterceptor } from '../../../../core/errors/error-interceptor';
import { Company } from '../../models/company.model';
import { CompaniesPage } from './companies-page';

describe('CompaniesPage', () => {
  let component: CompaniesPage;
  let fixture: ComponentFixture<CompaniesPage>;
  let httpMock: HttpTestingController;

  const sample: Company = {
    id: '1',
    name: 'Acme Widgets',
    website: 'https://acmewidgets.com/',
    websiteDomain: 'acmewidgets.com',
    createdAtUtc: '2026-01-01T00:00:00Z',
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CompaniesPage],
      providers: [
        provideHttpClient(withInterceptors([errorInterceptor])),
        provideHttpClientTesting(),
        provideRouter([]),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CompaniesPage);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('loads companies on init', () => {
    fixture.detectChanges();

    const req = httpMock.expectOne((r) => r.url.endsWith('/api/companies'));
    req.flush([sample]);

    expect(component.companies().length).toBe(1);
    expect(component.loading()).toBeFalse();
  });

  it('shows an error state when the load fails', () => {
    fixture.detectChanges();

    httpMock
      .expectOne((r) => r.url.endsWith('/api/companies'))
      .flush({ title: 'Boom' }, { status: 500, statusText: 'Server Error' });

    expect(component.loadError()).toContain('Something went wrong');
  });
});
