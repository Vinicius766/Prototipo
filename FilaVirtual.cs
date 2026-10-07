using System.Diagnostics;

int ingressos = 10;
int vendidos = 0;
object bloqueio = new object();
SemaphoreSlim filaVirtual = new SemaphoreSlim(3);   // só 3 pessoas por vez na tela de compra
Stopwatch relogio = Stopwatch.StartNew();

async Task ComprarAsync(int pessoa)
{
    await filaVirtual.WaitAsync();                  // entra na fila virtual (ou espera)
    try
    {
        await Task.Delay(500);                      // escolhendo assento e pagando

        lock (bloqueio)                             // reserva do ingresso: região crítica
        {
            int restantes = ingressos;              // ler
            Thread.Sleep(50);                       // simula um pequeno atraso

            if (restantes > 0)                      // verificar
            {
                ingressos = restantes - 1;          // gravar
                vendidos++;
                Console.WriteLine($"[{relogio.Elapsed.TotalSeconds:F1}s] Pessoa {pessoa} comprou");
            }
            else
            {
                Console.WriteLine($"[{relogio.Elapsed.TotalSeconds:F1}s] Pessoa {pessoa}: esgotado");
            }
        }
    }
    finally
    {
        filaVirtual.Release();                      // sai da fila, libera a vaga
    }
}

List<Task> compras = new List<Task>();
for (int i = 1; i <= 30; i++)
{
    compras.Add(ComprarAsync(i));
}
await Task.WhenAll(compras);

Console.WriteLine($"Ingressos vendidos: {vendidos}");
Console.WriteLine($"Tempo total: {relogio.Elapsed.TotalSeconds:F1}s");
