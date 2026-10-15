import { Component, inject, output, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AppError } from '../../../../core/errors/app-error';
import { Company } from '../../models/company.model';
import { CompanyService } from '../../services/company-service';

@Component({
  selector: 'app-company-create',
  imports: [ReactiveFormsModule],
  templateUrl: './company-create.html',
  styleUrl: './company-create.scss',
})
export class CompanyCreate {
  private readonly fb = inject(FormBuilder);
  private readonly companyService = inject(CompanyService);

  readonly created = output<Company>();

  readonly submitting = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(200)]],
    website: ['', [Validators.required, Validators.pattern(/^https?:\/\/.+/i)]],
  });

  submit(): void {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.companyService.create(this.form.getRawValue()).subscribe({
      next: (company) => {
        this.submitting.set(false);
        this.successMessage.set(`"${company.name}" was added successfully.`);
        this.form.reset();
        this.created.emit(company);
      },
      error: (error: AppError) => {
        this.submitting.set(false);
        this.errorMessage.set(error.userMessage);
      },
    });
  }
}
