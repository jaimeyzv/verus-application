import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, provideRouter } from '@angular/router';
import { errorInterceptor } from '../../../../core/errors/error-interceptor';
import { Company } from '../../models/company.model';
import { CompanyDetailPage } from './company-detail-page';

describe('CompanyDetailPage', () => {
  let component: CompanyDetailPage;
  let fixture: ComponentFixture<CompanyDetailPage>;
  let httpMock: HttpTestingController;

  const sample: Company = {
    id: '1',
    name: 'Acme Widgets',
    website: 'https://acmewidgets.com/',
    websiteDomain: 'acmewidgets.com',
    createdAtUtc: '2026-01-01T00:00:00Z',
  };

  function setup(id: string | null): void {
    TestBed.configureTestingModule({
      imports: [CompanyDetailPage],
      providers: [
        provideHttpClient(withInterceptors([errorInterceptor])),
        provideHttpClientTesting(),
        provideRouter([]),
        {
          provide: ActivatedRoute,
          useValue: { snapshot: { paramMap: convertToParamMap(id ? { id } : {}) } },
        },
      ],
    });
    fixture = TestBed.createComponent(CompanyDetailPage);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
  }

  afterEach(() => httpMock.verify());

  it('loads and shows the company for a valid id', () => {
    setup('1');
    fixture.detectChanges();

    httpMock.expectOne((r) => r.url.endsWith('/api/companies/1')).flush(sample);

    expect(component.company()?.name).toBe('Acme Widgets');
    expect(component.notFound()).toBeFalse();
  });

  it('shows a not-found state on a 404', () => {
    setup('999');
    fixture.detectChanges();

    httpMock
      .expectOne((r) => r.url.endsWith('/api/companies/999'))
      .flush(null, { status: 404, statusText: 'Not Found' });

    expect(component.notFound()).toBeTrue();
  });

  it('shows not-found when no id is present and makes no request', () => {
    setup(null);
    fixture.detectChanges();

    expect(component.notFound()).toBeTrue();
  });
});
