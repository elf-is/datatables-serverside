export class Customer {
  constructor(
    public customerNumber: number,
    public customerName: string,
    public contactLastName: string,
    public contactFirstName: string,
    public phone: string,
    public city: string,
    public state: string,
    public country: string,
    public creditLimit: number
  ) {
  }

}
