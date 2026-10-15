import { DatePipe } from '@angular/common';
import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Company } from '../../models/company.model';

@Component({
  selector: 'app-company-list',
  imports: [DatePipe, RouterLink],
  templateUrl: './company-list.html',
  styleUrl: './company-list.scss',
})
export class CompanyList {
  readonly companies = input.required<Company[]>();
  readonly loading = input<boolean>(false);
  readonly emptyMessage = input<string>('No companies yet.');
}
