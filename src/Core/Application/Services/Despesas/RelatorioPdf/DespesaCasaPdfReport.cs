﻿using Application.Helpers;
using Domain.Dtos.Despesas.Relatorios;
using Domain.Models.Membros;
using iText.Kernel.Pdf;
using iText.Layout;

namespace Application.Services.Despesas.RelatorioPdf
{
    public class DespesaCasaPdfReport
    {
        private readonly PdfTableHelper _pdfTable = new();

        public byte[] GerarRelatorioDespesaCasaPdf(DistribuicaoCustosCasaDto custosCasaDto)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new PdfWriter(memoryStream);
            using var mergedDocument = new PdfDocument(writer);
            using var doc = new Document(mergedDocument);

            _pdfTable.CreateTitleDocument(doc, "Relatório detalhado dos valores divididos");

            CreateTableCalculos(
                doc,
                custosCasaDto.TotalDespesaGerais,
                custosCasaDto.TotalAlmocoDividioComJhon,
                custosCasaDto.TotalAlmocoParteDoJhon,
                custosCasaDto.DespesaGeraisMaisAlmoco,
                custosCasaDto.Membros.Count
            );

            CreateTableValoresParaCada(
                doc,
                custosCasaDto.Membros,
                custosCasaDto.DespesaGeraisMaisAlmocoDividioPorMembro,
                custosCasaDto.TotalAlmocoParteDoJhon
            );

            doc.Close();

            return memoryStream.ToArray();
        }

        public void CreateTableCalculos(
            Document doc,
            double totalDespesaGeraisForaAlmoco,
            double totalAlmocoDividioComJhon,
            double totalAlmocoParteDoJhon,
            double despesaGeraisMaisAlmoco,
            int countMembros
        )
        {
            double totalAlmoco = totalAlmocoDividioComJhon + totalAlmocoParteDoJhon;

            var columnsValoresCalculados = new Dictionary<string, string>
            {
                { "Total de membros", $"{countMembros}" },
                { "Despesas fora almoço", $"R$ {totalDespesaGeraisForaAlmoco:F2}" },
                { "Despesas somente almoço", $"R$ {totalAlmoco:F2}" },
                { "Almoço fora parte do Jhon", $"R$ {totalAlmocoDividioComJhon:F2}" },
                { "Almoço parte do Jhon", $"R$ {totalAlmocoParteDoJhon:F2}" },
                { "Total das despesas com almoço", $"R$ {despesaGeraisMaisAlmoco:F2}" },
            };

            _pdfTable.CreateTable(
                doc,
                "Despesas da casa (Fora despesa Moradia como aluguel, luz etc...)",
                columnsValoresCalculados
            );
        }

        public void CreateTableValoresParaCada(
            Document doc,
            List<Membro> membros,
            double despesaGeraisMaisAlmocoDividioPorMembro,
            double totalAlmocoParteDoJhon
        )
        {
            Dictionary<string, string> columnsTotalParaCada = [];
            foreach(var membro in membros)
            {
                double valorParaCada = despesaGeraisMaisAlmocoDividioPorMembro;

                if(membro.Nome.Contains("Jhon", StringComparison.CurrentCultureIgnoreCase))
                {
                    valorParaCada = totalAlmocoParteDoJhon;
                }

                columnsTotalParaCada.Add(membro.Nome, $"{valorParaCada:F2}");
            }

            _pdfTable.CreateTable(doc, "Valor que cada um deve pagar", columnsTotalParaCada);
        }
    }
}