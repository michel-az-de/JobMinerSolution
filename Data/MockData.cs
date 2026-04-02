using System.Collections.Generic;
using JobMiner.Models;

namespace JobMiner.Data
{
 public static class MockData
 {
 public static List<Vaga> _vagasMock = new List<Vaga>
 {
 new Vaga { Id=1, Site="Apinfo", Titulo="Desenvolvedor .NET Senior — Azure", Responsavel="Carla Mendonça", Email="carla@techcorp.com.br", Telefone="(11)98877-6655", Url="https://apinfo.com.br/vagas/118721", Status=StatusVaga.Nova, Data="15/01", Match=95 },
 new Vaga { Id=2, Site="Apinfo", Titulo="Engenheiro Backend C# — Billing Platform", Responsavel="Ricardo Alves", Email="", Telefone="", Url="https://apinfo.com.br/vagas/118644", Status=StatusVaga.Pendente, Data="14/01", Match=88 },
 new Vaga { Id=3, Site="LinkedIn", Titulo="Senior Software Engineer .NET/Azure", Responsavel="Ana Beatriz Lima", Email="ana.lima@globaltech.io", Telefone="", Url="https://linkedin.com/jobs/3891047", Status=StatusVaga.Enviado, Data="13/01", Match=91 },
 new Vaga { Id=4, Site="Apinfo", Titulo="Desenvolvedor .NET8 — Backend — Remote", Responsavel="RH SoftwareLab", Email="rh@softwarelab.com.br", Telefone="(11)3245-1122", Url="https://apinfo.com.br/vagas/118390", Status=StatusVaga.Nova, Data="12/01", Match=80 },
 new Vaga { Id=5, Site="InfoJobs", Titulo="C# Developer Sr — Fintech100% Remote", Responsavel="Fernanda Costa", Email="fernanda@fintechcorp.com", Telefone="", Url="https://infojobs.com.br/vagas/99182", Status=StatusVaga.Pendente, Data="11/01", Match=85 },
 new Vaga { Id=6, Site="Apinfo", Titulo="Tech Lead .NET — Azure — Squad Internacional", Responsavel="Marcos Bittencourt", Email="marcos.b@avanade.com", Telefone="(11)97755-4433", Url="https://apinfo.com.br/vagas/118201", Status=StatusVaga.Enviado, Data="10/01", Match=98 },
 new Vaga { Id=7, Site="LinkedIn", Titulo="Backend Developer .NET — Startup SaaS", Responsavel="Paulo Salave'a", Email="", Telefone="", Url="https://linkedin.com/jobs/3845502", Status=StatusVaga.Excluida, Data="08/01", Match=62 },
 new Vaga { Id=8, Site="Apinfo", Titulo="Analista Desenvolvedor .NET Pleno/Sênior", Responsavel="HR Team FinanceBR", Email="vagas@financebr.com.br", Telefone="", Url="https://apinfo.com.br/vagas/117988", Status=StatusVaga.Pendente, Data="07/01", Match=73 },
 new Vaga { Id=9, Site="InfoJobs", Titulo="Arquiteto de Software .NET Cloud — Remote", Responsavel="Gabriela Rocha", Email="gabriela@cloudsoft.io", Telefone="(21)99001-3344", Url="https://infojobs.com.br/vagas/98741", Status=StatusVaga.Nova, Data="06/01", Match=79 }
 };
 }
}