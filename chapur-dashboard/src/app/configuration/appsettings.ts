import { environment } from '../../environments/environment.dev';

export class Appsettings {
  public static VERSION = environment.version;
  public static ESLACTIC = environment.isElasticBenstalk;
  public static API_ENDPOINT = environment.isElasticBenstalk ?  `${environment.apiUrl}/${Appsettings.VERSION}` :
                              `${environment.apiUrl}`;
  public static API_ENDPOINT_FULL = environment.isElasticBenstalk ?  `${environment.apiUrl}/${Appsettings.VERSION}` :
                                   `${environment.apiUrl}/api/${Appsettings.VERSION}`;


}
