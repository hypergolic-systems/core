import { HttpClient } from "@angular/common/http";
import { Injectable, computed, inject } from "@angular/core";
import { toSignal } from "@angular/core/rxjs-interop";
import { Observable, catchError, exhaustMap, interval, map, of } from "rxjs";
import * as zod from 'zod';

export const Battery = zod.object({
  type: zod.literal('Battery'),
  flow: zod.number(),
  stored: zod.number(),
  capacity: zod.number(),
  canProduce: zod.number(),
  canConsume: zod.number(),
});
export type Battery = zod.infer<typeof Battery>;

export const RadioisotopeThermalGenerator = zod.object({
  type: zod.literal('RadioisotopeThermalGenerator'),
  flow: zod.number(),
  canProduce: zod.number(),
});
export type RadioisotopeThermalGenerator = zod.infer<typeof RadioisotopeThermalGenerator>;

export const VirtualComponent = zod.union([Battery, RadioisotopeThermalGenerator]);
export type VirtualComponent = zod.infer<typeof VirtualComponent>;

export const Module = zod.object({
  type: zod.string(),
  name: zod.string(),
  virtualComponents: zod.optional(zod.array(VirtualComponent)),
});
export type Module = zod.infer<typeof Module>;

export const Part = zod.object({
  id: zod.string(),
  name: zod.string(),
  type: zod.string(),
  modules: zod.optional(zod.array(Module)),
});
export type Part = zod.infer<typeof Part>;

export const Vessel = zod.object({
  id: zod.string(),
  name: zod.string(),
  type: zod.string(),
  parts: zod.optional(zod.array(Part)),
});
export type Vessel = zod.infer<typeof Vessel>;

const Status = zod.object({
  "vessels": zod.array(Vessel),
});
type Status = zod.infer<typeof Status>;

@Injectable({providedIn: 'root'})
export class KspClient {
  private http = inject(HttpClient);

  private readonly status = toSignal(
    interval(1000).pipe(
      exhaustMap(() => this.get('/status', Status)
      ))
  );

  readonly vessels = computed(() => this.status()?.vessels.filter(v => v.type !== 'SpaceObject') ?? []);



  private get<T>(url: string, schema: zod.Schema<T>): Observable<T|undefined> {
    return this.http.get(`http://localhost:3456${url}`).pipe(
      map(res => schema.parse(res)),
      catchError(err => {
        if (err instanceof zod.ZodError) {
          err.issues.forEach(issue => console.error(issue));
        }
        console.error(err);
        return of(undefined);
      }),
    );
  }
}