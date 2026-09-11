import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { RutinaDetalle, RutinaGuardar, RutinaResumen } from '../models/rutina';

// ------------------------------------------------------------
// RutinaService: TODA la comunicación con /api/rutinas
// ------------------------------------------------------------
// Un SERVICIO es una clase sin pantalla con lógica reutilizable. Los
// componentes se ocupan de MOSTRAR; los servicios, de TRAER y MANDAR datos.
// Es el mismo rol que RutinaService en el backend.
//
// @Injectable({ providedIn: 'root' }) registra el servicio en el inyector
// principal de la app y crea UNA SOLA instancia compartida (singleton).
// Es como hacer builder.Services.AddSingleton<RutinaService>() en .NET,
// pero sin tener que escribirlo en ningún otro archivo.
@Injectable({ providedIn: 'root' })
export class RutinaService {
  // inject() pide una dependencia al contenedor de Angular.
  // Equivale a recibir HttpClient por el constructor en C#.
  // HttpClient está disponible porque lo registramos con provideHttpClient()
  // en app.config.ts.
  private readonly http = inject(HttpClient);

  // `...${}...` es un "template string": como $"...{}..." en C#.
  // environment.apiUrl vale 'http://localhost:5000/api'.
  private readonly url = `${environment.apiUrl}/rutinas`;

  // ---------------------------------------------------------------------
  // IMPORTANTE: estos métodos NO hacen la petición HTTP. Devuelven un
  // Observable, que es una "receta": la petición sale recién cuando alguien
  // hace .subscribe(). Si llamás listar() y no te suscribís, no pasa nada.
  //
  // El genérico <RutinaResumen[]> le dice a TypeScript qué va a llegar.
  // No lo valida en tiempo de ejecución, pero da autocompletado y errores
  // de compilación si usamos mal los datos.
  // ---------------------------------------------------------------------

  // GET /api/rutinas
  listar(): Observable<RutinaResumen[]> {
    return this.http.get<RutinaResumen[]>(this.url);
  }

  // GET /api/rutinas/buscar?nombre=texto
  buscar(nombre: string): Observable<RutinaResumen[]> {
    // HttpParams arma el query string y escapa caracteres especiales
    // (espacios, "&", "/", tildes...). Nunca concatenes "?nombre=" + texto a mano.
    const params = new HttpParams().set('nombre', nombre);
    return this.http.get<RutinaResumen[]>(`${this.url}/buscar`, { params });
  }

  // GET /api/rutinas/5
  obtener(id: number): Observable<RutinaDetalle> {
    return this.http.get<RutinaDetalle>(`${this.url}/${id}`);
  }

  // POST /api/rutinas  (el segundo parámetro es el body; se convierte a JSON solo)
  crear(rutina: RutinaGuardar): Observable<RutinaDetalle> {
    return this.http.post<RutinaDetalle>(this.url, rutina);
  }

  // PUT /api/rutinas/5
  actualizar(id: number, rutina: RutinaGuardar): Observable<RutinaDetalle> {
    return this.http.put<RutinaDetalle>(`${this.url}/${id}`, rutina);
  }

  // DELETE /api/rutinas/5  (la API responde 204 sin contenido -> void)
  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
