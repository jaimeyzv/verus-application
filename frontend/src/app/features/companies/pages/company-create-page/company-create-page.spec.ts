import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { errorInterceptor } from '../../../../core/errors/error-interceptor';
import { Company } from '../../models/company.model';
import { CompanyCreatePage } from './company-create-page';

describe('CompanyCreatePage', () => {
  let component: CompanyCreatePage;
  let fixture: ComponentFixture<CompanyCreatePage>;

  const sample: Company = {
    id: 'abc',
    name: 'Acme Widgets',
    website: 'https://acmewidgets.com/',
    websiteDomain: 'acmewidgets.com',
    createdAtUtc: '2026-01-01T00:00:00Z',
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CompanyCreatePage],
      providers: [
        provideHttpClient(withInterceptors([errorInterceptor])),
        provideHttpClientTesting(),
        provideRouter([]),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CompanyCreatePage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('navigates to the new company detail on creation', () => {
    const router = TestBed.inject(Router);
    const navigateSpy = spyOn(router, 'navigate');

    component.onCreated(sample);

    expect(navigateSpy).toHaveBeenCalledWith(['/companies', sample.id]);
  });
});
