import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../../environments/environment';
import { Company } from '../models/company.model';
import { CompanyService } from './company-service';

describe('CompanyService', () => {
  let service: CompanyService;
  let httpMock: HttpTestingController;
  const apiUrl = `${environment.apiBaseUrl}/api/companies`;

  const sampleCompany: Company = {
    id: '11111111-1111-1111-1111-111111111111',
    name: 'Acme Widgets',
    website: 'https://acmewidgets.com/',
    websiteDomain: 'acmewidgets.com',
    createdAtUtc: '2026-01-01T00:00:00Z',
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(CompanyService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('getAll issues a GET to the companies endpoint', () => {
    service.getAll().subscribe((companies) => {
      expect(companies).toEqual([sampleCompany]);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush([sampleCompany]);
  });

  it('create issues a POST with the request body', () => {
    const request = { name: 'Acme Widgets', website: 'https://acmewidgets.com' };

    service.create(request).subscribe((company) => {
      expect(company).toEqual(sampleCompany);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(request);
    req.flush(sampleCompany);
  });

  it('search appends only the provided query parameters', () => {
    service.search({ term: 'acme' }).subscribe();

    const req = httpMock.expectOne((r) => r.url === `${apiUrl}/search`);
    expect(req.request.params.get('term')).toBe('acme');
    expect(req.request.params.has('name')).toBeFalse();
    req.flush([]);
  });
});
