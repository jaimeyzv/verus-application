import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AppError } from '../../../../core/errors/app-error';
import { CompanyList } from '../../components/company-list/company-list';
import { Company } from '../../models/company.model';
import { CompanyService } from '../../services/company-service';

@Component({
  selector: 'app-company-search-page',
  imports: [ReactiveFormsModule, CompanyList, RouterLink],
  templateUrl: './company-search-page.html',
  styleUrl: './company-search-page.scss',
})
export class CompanySearchPage {
  private readonly fb = inject(FormBuilder);
  private readonly companyService = inject(CompanyService);

  readonly results = signal<Company[]>([]);
  readonly loading = signal(false);
  readonly searched = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    name: [''],
    domain: [''],
    term: [''],
  });

  search(): void {
    const { name, domain, term } = this.form.getRawValue();
    const criteria = {
      name: name.trim() || undefined,
      domain: domain.trim() || undefined,
      term: term.trim() || undefined,
    };

    this.loading.set(true);
    this.errorMessage.set(null);

    this.companyService.search(criteria).subscribe({
      next: (companies) => {
        this.results.set(companies);
        this.searched.set(true);
        this.loading.set(false);
      },
      error: (error: AppError) => {
        this.errorMessage.set(error.userMessage);
        this.loading.set(false);
      },
    });
  }

  reset(): void {
    this.form.reset();
    this.results.set([]);
    this.searched.set(false);
    this.errorMessage.set(null);
  }
}
