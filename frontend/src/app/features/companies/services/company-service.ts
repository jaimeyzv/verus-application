import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Company, CreateCompanyRequest } from '../models/company.model';

@Injectable({ providedIn: 'root' })
export class CompanyService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiBaseUrl}/api/companies`;

  getAll(): Observable<Company[]> {
    return this.http.get<Company[]>(this.apiUrl);
  }

  getById(id: string): Observable<Company> {
    return this.http.get<Company>(`${this.apiUrl}/${id}`);
  }

  search(criteria: { name?: string; domain?: string; term?: string }): Observable<Company[]> {
    let params = new HttpParams();
    if (criteria.name) {
      params = params.set('name', criteria.name);
    }
    if (criteria.domain) {
      params = params.set('domain', criteria.domain);
    }
    if (criteria.term) {
      params = params.set('term', criteria.term);
    }

    return this.http.get<Company[]>(`${this.apiUrl}/search`, { params });
  }

  create(request: CreateCompanyRequest): Observable<Company> {
    return this.http.post<Company>(this.apiUrl, request);
  }
}
