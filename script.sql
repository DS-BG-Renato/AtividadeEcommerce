create	 database bdEcommerce;

use bdEcommerce;

create table Usuario(
	Id int primary key auto_increment,
    Nome varchar(50) not null,
    Email varchar(50) not null,
    Senha varchar(50) not null
);

create table Cliente(
	CodCli int primary key auto_increment,
    NomeCli varchar(50) not null,
    TelCli varchar(20) not null,
    EmailCli varchar(50) not null
);

create table Produto(
	CodProd int primary key auto_increment,
    NomeProd varchar(50) not null,
    DescricaoProd varchar(50) not null,
    QuantidadeProd int,
    PrecoProd float
);

select * from Usuario;
select * from Cliente;
select * from Produto;

insert into Usuario(Nome, Email, Senha) values('admin', 'admin@email.com', '12345678');