import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CompanyCreate } from '../../components/company-create/company-create';
import { Company } from '../../models/company.model';

@Component({
  selector: 'app-company-create-page',
  imports: [CompanyCreate, RouterLink],
  templateUrl: './company-create-page.html',
  styleUrl: './company-create-page.scss',
})
export class CompanyCreatePage {
  private readonly router = inject(Router);

  onCreated(company: Company): void {
    this.router.navigate(['/companies', company.id]);
  }
}
