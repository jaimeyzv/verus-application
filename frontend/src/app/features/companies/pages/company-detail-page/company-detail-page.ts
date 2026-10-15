import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AppError } from '../../../../core/errors/app-error';
import { Company } from '../../models/company.model';
import { CompanyService } from '../../services/company-service';

@Component({
  selector: 'app-company-detail-page',
  imports: [DatePipe, RouterLink],
  templateUrl: './company-detail-page.html',
  styleUrl: './company-detail-page.scss',
})
export class CompanyDetailPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly companyService = inject(CompanyService);

  readonly company = signal<Company | null>(null);
  readonly loading = signal(false);
  readonly notFound = signal(false);
  readonly errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.notFound.set(true);
      return;
    }
    this.load(id);
  }

  load(id: string): void {
    this.loading.set(true);
    this.notFound.set(false);
    this.errorMessage.set(null);

    this.companyService.getById(id).subscribe({
      next: (company) => {
        this.company.set(company);
        this.loading.set(false);
      },
      error: (error: AppError) => {
        this.loading.set(false);
        if (error.status === 404) {
          this.notFound.set(true);
        } else {
          this.errorMessage.set(error.userMessage);
        }
      },
    });
  }
}
