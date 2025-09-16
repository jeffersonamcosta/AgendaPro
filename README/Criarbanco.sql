/* ===============================
   CRIAÇÃO DO BANCO
================================= */
--CREATE DATABASE AgendaPro;
GO

USE AgendaPro;
GO

/* ===============================
   TABELA: Tipos de Evento
   (classificação: palestra, workshop, show, etc.)
================================= */
CREATE TABLE TiposEvento (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Descricao NVARCHAR(200) NOT NULL,
    Ativo BIT NOT NULL DEFAULT 1
);
GO

/* ===============================
   TABELA: Eventos
================================= */
CREATE TABLE Eventos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(200) NOT NULL,
    DataInicio DATETIME2 NOT NULL,
    DataFim DATETIME2 NOT NULL,
    Endereco NVARCHAR(400) NULL,
    Observacoes NVARCHAR(MAX) NULL,
    CapacidadeMaxima INT NOT NULL,
    OrcamentoMaximo DECIMAL(18,2) NOT NULL,
    TipoEventoId INT NOT NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Eventos_TiposEvento FOREIGN KEY (TipoEventoId) REFERENCES TiposEvento(Id)
);
GO

/* ===============================
   TABELA: Participantes
   (pessoas que participam dos eventos)
================================= */
CREATE TABLE Participantes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(200) NOT NULL,
    Documento NVARCHAR(14) NOT NULL UNIQUE, -- CPF
    Telefone NVARCHAR(30) NULL,
    Email NVARCHAR(200) NULL,
    TipoParticipante INT NOT NULL, -- 0=Normal, 1=VIP
    Ativo BIT NOT NULL DEFAULT 1
);
GO

/* ===============================
   TABELA: Fornecedores
   (empresas ou profissionais que prestam serviços)
================================= */
CREATE TABLE Fornecedores (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RazaoSocial NVARCHAR(200) NOT NULL,
    CNPJ NVARCHAR(14) NOT NULL UNIQUE,
    Telefone NVARCHAR(30) NULL,
    Email NVARCHAR(200) NULL,
    Ativo BIT NOT NULL DEFAULT 1
);
GO

/* ===============================
   TABELA: Serviços
   (cada fornecedor pode oferecer vários serviços)
================================= */
CREATE TABLE Servicos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FornecedorId INT NOT NULL,
    Nome NVARCHAR(200) NOT NULL,
    Preco DECIMAL(18,2) NOT NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Servicos_Fornecedores FOREIGN KEY (FornecedorId) REFERENCES Fornecedores(Id)
);
GO

/* ===============================
   TABELA: Associação Participante ↔ Evento
================================= */
CREATE TABLE ParticipanteEvento (
    ParticipanteId INT NOT NULL,
    EventoId INT NOT NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    CONSTRAINT PK_ParticipanteEvento PRIMARY KEY (ParticipanteId, EventoId),
    CONSTRAINT FK_ParticipanteEvento_Participante FOREIGN KEY (ParticipanteId) REFERENCES Participantes(Id),
    CONSTRAINT FK_ParticipanteEvento_Evento FOREIGN KEY (EventoId) REFERENCES Eventos(Id)
);
GO

/* ===============================
   TABELA: Associação Fornecedor ↔ Evento
   (quais fornecedores foram contratados para o evento)
================================= */
CREATE TABLE FornecedorEvento (
    FornecedorId INT NOT NULL,
    EventoId INT NOT NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    CONSTRAINT PK_FornecedorEvento PRIMARY KEY (FornecedorId, EventoId),
    CONSTRAINT FK_FornecedorEvento_Fornecedor FOREIGN KEY (FornecedorId) REFERENCES Fornecedores(Id),
    CONSTRAINT FK_FornecedorEvento_Evento FOREIGN KEY (EventoId) REFERENCES Eventos(Id)
);
GO

CREATE TABLE Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL,
    Login NVARCHAR(30) NOT NULL UNIQUE,
    SenhaHash NVARCHAR(200) NOT NULL, -- hash compacto
    Perfil NVARCHAR(20) NOT NULL,     -- Admin, Gestor, Operador
    DataCriacao DATETIME2 NOT NULL DEFAULT GETDATE(),
    Ativo BIT NOT NULL DEFAULT 1
);
GO

/* ===============================
   ÍNDICES RECOMENDADOS
================================= */
CREATE INDEX IX_Participantes_Documento ON Participantes(Documento);
CREATE INDEX IX_Fornecedores_CNPJ ON Fornecedores(CNPJ);
CREATE INDEX IX_Eventos_Datas ON Eventos(DataInicio, DataFim);
