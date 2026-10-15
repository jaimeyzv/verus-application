import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AppError } from '../../../../core/errors/app-error';
import { CompanyList } from '../../components/company-list/company-list';
import { Company } from '../../models/company.model';
import { CompanyService } from '../../services/company-service';

@Component({
  selector: 'app-companies-page',
  imports: [CompanyList, RouterLink],
  templateUrl: './companies-page.html',
  styleUrl: './companies-page.scss',
})
export class CompaniesPage implements OnInit {
  private readonly companyService = inject(CompanyService);

  readonly companies = signal<Company[]>([]);
  readonly loading = signal(false);
  readonly loadError = signal<string | null>(null);

  ngOnInit(): void {
    this.loadCompanies();
  }

  loadCompanies(): void {
    this.loading.set(true);
    this.loadError.set(null);

    this.companyService.getAll().subscribe({
      next: (companies) => {
        this.companies.set(companies);
        this.loading.set(false);
      },
      error: (error: AppError) => {
        this.loadError.set(error.userMessage);
        this.loading.set(false);
      },
    });
  }
}
