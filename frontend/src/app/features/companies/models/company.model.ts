export interface Company {
  id: string;
  name: string;
  website: string;
  websiteDomain: string;
  createdAtUtc: string;
}

export interface CreateCompanyRequest {
  name: string;
  website: string;
}
