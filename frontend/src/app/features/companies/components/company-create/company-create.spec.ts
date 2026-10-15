import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { errorInterceptor } from '../../../../core/errors/error-interceptor';
import { CompanyCreate } from './company-create';

describe('CompanyCreate', () => {
  let component: CompanyCreate;
  let fixture: ComponentFixture<CompanyCreate>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CompanyCreate],
      providers: [
        provideHttpClient(withInterceptors([errorInterceptor])),
        provideHttpClientTesting(),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CompanyCreate);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
  });

  afterEach(() => httpMock.verify());

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('does not call the API when the form is empty', () => {
    component.submit();

    httpMock.expectNone(() => true);
    expect(component.form.touched).toBeTrue();
  });

  it('emits created and shows a success message on success', () => {
    const createdSpy = jasmine.createSpy('created');
    component.created.subscribe(createdSpy);
    component.form.setValue({ name: 'Acme Widgets', website: 'https://acmewidgets.com' });

    component.submit();

    const req = httpMock.expectOne((r) => r.url.endsWith('/api/companies') && r.method === 'POST');
    req.flush({
      id: '1',
      name: 'Acme Widgets',
      website: 'https://acmewidgets.com/',
      websiteDomain: 'acmewidgets.com',
      createdAtUtc: '2026-01-01T00:00:00Z',
    });

    expect(createdSpy).toHaveBeenCalled();
    expect(component.successMessage()).toContain('Acme Widgets');
  });

  it('shows the backend validation message on a 400 response', () => {
    component.form.setValue({ name: 'Acme Widgets', website: 'https://totallyunrelated.com' });

    component.submit();

    const req = httpMock.expectOne((r) => r.url.endsWith('/api/companies'));
    req.flush(
      {
        title: 'One or more validation errors occurred.',
        errors: { Website: ['does not appear relevant'] },
      },
      { status: 400, statusText: 'Bad Request' },
    );

    expect(component.errorMessage()).toBe('does not appear relevant');
  });
});
