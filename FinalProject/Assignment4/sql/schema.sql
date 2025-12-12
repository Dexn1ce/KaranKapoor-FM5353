--
-- PostgreSQL database dump
--

\restrict DfCR84ZJv1e5qBsLZGD7srgLony7pvwAx5JGpvYI7VXHRzmrgHlc70VdwjW4FhQ

-- Dumped from database version 14.20 (Ubuntu 14.20-0ubuntu0.22.04.1)
-- Dumped by pg_dump version 14.20 (Ubuntu 14.20-0ubuntu0.22.04.1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: karankapoor
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO karankapoor;

--
-- Name: asian_option; Type: TABLE; Schema: public; Owner: karankapoor
--

CREATE TABLE public.asian_option (
    id integer NOT NULL,
    vol double precision,
    strike double precision,
    b double precision,
    otyp character varying(5),
    expirydate date,
    ratecurveid integer,
    underlyingid integer
);


ALTER TABLE public.asian_option OWNER TO karankapoor;

--
-- Name: asian_option_id_seq; Type: SEQUENCE; Schema: public; Owner: karankapoor
--

CREATE SEQUENCE public.asian_option_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.asian_option_id_seq OWNER TO karankapoor;

--
-- Name: asian_option_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: karankapoor
--

ALTER SEQUENCE public.asian_option_id_seq OWNED BY public.asian_option.id;


--
-- Name: barrier_option; Type: TABLE; Schema: public; Owner: karankapoor
--

CREATE TABLE public.barrier_option (
    id integer NOT NULL,
    vol double precision,
    b double precision,
    otyp character varying(5),
    strike double precision,
    barrierlevel double precision,
    barriertype character varying(15),
    ratecurveid integer,
    underlyingid integer,
    expirydate date
);


ALTER TABLE public.barrier_option OWNER TO karankapoor;

--
-- Name: barrier_option_id_seq; Type: SEQUENCE; Schema: public; Owner: karankapoor
--

CREATE SEQUENCE public.barrier_option_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.barrier_option_id_seq OWNER TO karankapoor;

--
-- Name: barrier_option_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: karankapoor
--

ALTER SEQUENCE public.barrier_option_id_seq OWNED BY public.barrier_option.id;


--
-- Name: digital_option; Type: TABLE; Schema: public; Owner: karankapoor
--

CREATE TABLE public.digital_option (
    id integer NOT NULL,
    vol double precision,
    strike double precision,
    b double precision,
    otyp character varying(5),
    payout_amount double precision,
    expirydate date,
    underlyingid integer,
    ratecurveid integer
);


ALTER TABLE public.digital_option OWNER TO karankapoor;

--
-- Name: digital_option_id_seq; Type: SEQUENCE; Schema: public; Owner: karankapoor
--

CREATE SEQUENCE public.digital_option_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.digital_option_id_seq OWNER TO karankapoor;

--
-- Name: digital_option_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: karankapoor
--

ALTER SEQUENCE public.digital_option_id_seq OWNED BY public.digital_option.id;


--
-- Name: exchange; Type: TABLE; Schema: public; Owner: karankapoor
--

CREATE TABLE public.exchange (
    exchangeid integer NOT NULL,
    name character varying(100) NOT NULL,
    country character varying(50),
    createdat timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updatedat timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public.exchange OWNER TO karankapoor;

--
-- Name: exchange_exchangeid_seq; Type: SEQUENCE; Schema: public; Owner: karankapoor
--

ALTER TABLE public.exchange ALTER COLUMN exchangeid ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.exchange_exchangeid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: historicalprice; Type: TABLE; Schema: public; Owner: karankapoor
--

CREATE TABLE public.historicalprice (
    historicalpriceid integer NOT NULL,
    underlyingid integer NOT NULL,
    pricetime timestamp without time zone NOT NULL,
    lastprice double precision NOT NULL
);


ALTER TABLE public.historicalprice OWNER TO karankapoor;

--
-- Name: historicalprice_historicalpriceid_seq; Type: SEQUENCE; Schema: public; Owner: karankapoor
--

ALTER TABLE public.historicalprice ALTER COLUMN historicalpriceid ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.historicalprice_historicalpriceid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: lookback_option; Type: TABLE; Schema: public; Owner: karankapoor
--

CREATE TABLE public.lookback_option (
    id integer NOT NULL,
    vol double precision,
    strike double precision,
    b double precision,
    otyp character varying(5),
    expirydate date,
    ratecurveid integer,
    underlyingid integer
);


ALTER TABLE public.lookback_option OWNER TO karankapoor;

--
-- Name: lookback_option_id_seq; Type: SEQUENCE; Schema: public; Owner: karankapoor
--

CREATE SEQUENCE public.lookback_option_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.lookback_option_id_seq OWNER TO karankapoor;

--
-- Name: lookback_option_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: karankapoor
--

ALTER SEQUENCE public.lookback_option_id_seq OWNED BY public.lookback_option.id;


--
-- Name: market; Type: TABLE; Schema: public; Owner: karankapoor
--

CREATE TABLE public.market (
    marketid integer NOT NULL,
    exchangeid integer NOT NULL,
    name character varying(100),
    type character varying(50)
);


ALTER TABLE public.market OWNER TO karankapoor;

--
-- Name: market_marketid_seq; Type: SEQUENCE; Schema: public; Owner: karankapoor
--

ALTER TABLE public.market ALTER COLUMN marketid ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.market_marketid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: mytable; Type: TABLE; Schema: public; Owner: karankapoor
--

CREATE TABLE public.mytable (
    curvedate date,
    tenor_1m double precision,
    tenor_1_5m double precision,
    tenor_2m double precision,
    tenor_3m double precision,
    tenor_4m double precision,
    tenor_6m double precision,
    tenor_1y double precision,
    tenor_2y double precision,
    tenor_3y double precision,
    tenor_5y double precision,
    tenor_7y double precision,
    tenor_10y double precision,
    tenor_20y double precision,
    tenor_30y double precision
);


ALTER TABLE public.mytable OWNER TO karankapoor;

--
-- Name: option; Type: TABLE; Schema: public; Owner: karankapoor
--

CREATE TABLE public.option (
    id integer NOT NULL,
    vol double precision,
    strike double precision,
    b double precision,
    otyp character varying(5),
    ratecurveid integer,
    expirydate date,
    underlyingid integer
);


ALTER TABLE public.option OWNER TO karankapoor;

--
-- Name: option_id_seq; Type: SEQUENCE; Schema: public; Owner: karankapoor
--

CREATE SEQUENCE public.option_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.option_id_seq OWNER TO karankapoor;

--
-- Name: option_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: karankapoor
--

ALTER SEQUENCE public.option_id_seq OWNED BY public.option.id;


--
-- Name: range_option; Type: TABLE; Schema: public; Owner: karankapoor
--

CREATE TABLE public.range_option (
    id integer NOT NULL,
    vol double precision,
    b double precision,
    underlyingid integer,
    expirydate date,
    ratecurveid integer
);


ALTER TABLE public.range_option OWNER TO karankapoor;

--
-- Name: range_option_id_seq; Type: SEQUENCE; Schema: public; Owner: karankapoor
--

CREATE SEQUENCE public.range_option_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.range_option_id_seq OWNER TO karankapoor;

--
-- Name: range_option_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: karankapoor
--

ALTER SEQUENCE public.range_option_id_seq OWNED BY public.range_option.id;


--
-- Name: ratecurve; Type: TABLE; Schema: public; Owner: karankapoor
--

CREATE TABLE public.ratecurve (
    ratecurveid integer NOT NULL,
    curvename character varying(100),
    currency character varying(10),
    curvedate date NOT NULL
);


ALTER TABLE public.ratecurve OWNER TO karankapoor;

--
-- Name: ratecurve_ratecurveid_seq; Type: SEQUENCE; Schema: public; Owner: karankapoor
--

ALTER TABLE public.ratecurve ALTER COLUMN ratecurveid ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.ratecurve_ratecurveid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: ratepoint; Type: TABLE; Schema: public; Owner: karankapoor
--

CREATE TABLE public.ratepoint (
    ratepointid integer NOT NULL,
    ratecurveid integer NOT NULL,
    tenor double precision NOT NULL,
    rate double precision NOT NULL
);


ALTER TABLE public.ratepoint OWNER TO karankapoor;

--
-- Name: ratepoint_ratepointid_seq; Type: SEQUENCE; Schema: public; Owner: karankapoor
--

ALTER TABLE public.ratepoint ALTER COLUMN ratepointid ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.ratepoint_ratepointid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: trade; Type: TABLE; Schema: public; Owner: karankapoor
--

CREATE TABLE public.trade (
    tradeid integer NOT NULL,
    underlyingid integer NOT NULL,
    marketid integer NOT NULL,
    direction character varying(4) NOT NULL,
    quantity double precision NOT NULL,
    tradeprice double precision NOT NULL,
    tradetime timestamp with time zone NOT NULL
);


ALTER TABLE public.trade OWNER TO karankapoor;

--
-- Name: trade_tradeid_seq; Type: SEQUENCE; Schema: public; Owner: karankapoor
--

CREATE SEQUENCE public.trade_tradeid_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.trade_tradeid_seq OWNER TO karankapoor;

--
-- Name: trade_tradeid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: karankapoor
--

ALTER SEQUENCE public.trade_tradeid_seq OWNED BY public.trade.tradeid;


--
-- Name: underlying; Type: TABLE; Schema: public; Owner: karankapoor
--

CREATE TABLE public.underlying (
    underlyingid integer NOT NULL,
    symbol character varying(20) NOT NULL,
    name character varying(100),
    assettype character varying(20)
);


ALTER TABLE public.underlying OWNER TO karankapoor;

--
-- Name: underlying_underlyingid_seq; Type: SEQUENCE; Schema: public; Owner: karankapoor
--

ALTER TABLE public.underlying ALTER COLUMN underlyingid ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.underlying_underlyingid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: underlyingmarket; Type: TABLE; Schema: public; Owner: karankapoor
--

CREATE TABLE public.underlyingmarket (
    underlyingid integer NOT NULL,
    marketid integer NOT NULL
);


ALTER TABLE public.underlyingmarket OWNER TO karankapoor;

--
-- Name: underlyingtable; Type: TABLE; Schema: public; Owner: karankapoor
--

CREATE TABLE public.underlyingtable (
    date date NOT NULL,
    close double precision,
    open double precision,
    high double precision,
    low double precision,
    volume double precision,
    symbol character varying(10),
    underlyingid integer NOT NULL
);


ALTER TABLE public.underlyingtable OWNER TO karankapoor;

--
-- Name: underlyingtable_underlyingid_seq; Type: SEQUENCE; Schema: public; Owner: karankapoor
--

ALTER TABLE public.underlyingtable ALTER COLUMN underlyingid ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.underlyingtable_underlyingid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: asian_option id; Type: DEFAULT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.asian_option ALTER COLUMN id SET DEFAULT nextval('public.asian_option_id_seq'::regclass);


--
-- Name: barrier_option id; Type: DEFAULT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.barrier_option ALTER COLUMN id SET DEFAULT nextval('public.barrier_option_id_seq'::regclass);


--
-- Name: digital_option id; Type: DEFAULT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.digital_option ALTER COLUMN id SET DEFAULT nextval('public.digital_option_id_seq'::regclass);


--
-- Name: lookback_option id; Type: DEFAULT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.lookback_option ALTER COLUMN id SET DEFAULT nextval('public.lookback_option_id_seq'::regclass);


--
-- Name: option id; Type: DEFAULT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.option ALTER COLUMN id SET DEFAULT nextval('public.option_id_seq'::regclass);


--
-- Name: range_option id; Type: DEFAULT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.range_option ALTER COLUMN id SET DEFAULT nextval('public.range_option_id_seq'::regclass);


--
-- Name: trade tradeid; Type: DEFAULT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.trade ALTER COLUMN tradeid SET DEFAULT nextval('public.trade_tradeid_seq'::regclass);


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: asian_option asian_option_pkey; Type: CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.asian_option
    ADD CONSTRAINT asian_option_pkey PRIMARY KEY (id);


--
-- Name: barrier_option barrier_option_pkey; Type: CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.barrier_option
    ADD CONSTRAINT barrier_option_pkey PRIMARY KEY (id);


--
-- Name: digital_option digital_option_pkey; Type: CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.digital_option
    ADD CONSTRAINT digital_option_pkey PRIMARY KEY (id);


--
-- Name: exchange exchange_pkey; Type: CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.exchange
    ADD CONSTRAINT exchange_pkey PRIMARY KEY (exchangeid);


--
-- Name: historicalprice historicalprice_pkey; Type: CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.historicalprice
    ADD CONSTRAINT historicalprice_pkey PRIMARY KEY (historicalpriceid);


--
-- Name: historicalprice historicalprice_underlyingid_pricetime_key; Type: CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.historicalprice
    ADD CONSTRAINT historicalprice_underlyingid_pricetime_key UNIQUE (underlyingid, pricetime);


--
-- Name: lookback_option lookback_option_pkey; Type: CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.lookback_option
    ADD CONSTRAINT lookback_option_pkey PRIMARY KEY (id);


--
-- Name: market market_pkey; Type: CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.market
    ADD CONSTRAINT market_pkey PRIMARY KEY (marketid);


--
-- Name: option option_pkey; Type: CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.option
    ADD CONSTRAINT option_pkey PRIMARY KEY (id);


--
-- Name: range_option range_option_pkey; Type: CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.range_option
    ADD CONSTRAINT range_option_pkey PRIMARY KEY (id);


--
-- Name: ratecurve ratecurve_pkey; Type: CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.ratecurve
    ADD CONSTRAINT ratecurve_pkey PRIMARY KEY (ratecurveid);


--
-- Name: ratepoint ratepoint_pkey; Type: CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.ratepoint
    ADD CONSTRAINT ratepoint_pkey PRIMARY KEY (ratepointid);


--
-- Name: trade trade_pkey; Type: CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.trade
    ADD CONSTRAINT trade_pkey PRIMARY KEY (tradeid);


--
-- Name: underlying underlying_pkey; Type: CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.underlying
    ADD CONSTRAINT underlying_pkey PRIMARY KEY (underlyingid);


--
-- Name: underlyingmarket underlyingmarket_pkey; Type: CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.underlyingmarket
    ADD CONSTRAINT underlyingmarket_pkey PRIMARY KEY (underlyingid, marketid);


--
-- Name: underlyingtable underlyingtable_pkey; Type: CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.underlyingtable
    ADD CONSTRAINT underlyingtable_pkey PRIMARY KEY (underlyingid);


--
-- Name: barrier_option fk_barrier_ratecurve; Type: FK CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.barrier_option
    ADD CONSTRAINT fk_barrier_ratecurve FOREIGN KEY (ratecurveid) REFERENCES public.ratecurve(ratecurveid);


--
-- Name: range_option fk_barrier_ratecurve; Type: FK CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.range_option
    ADD CONSTRAINT fk_barrier_ratecurve FOREIGN KEY (ratecurveid) REFERENCES public.ratecurve(ratecurveid);


--
-- Name: barrier_option fk_barrier_underlying; Type: FK CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.barrier_option
    ADD CONSTRAINT fk_barrier_underlying FOREIGN KEY (underlyingid) REFERENCES public.underlying(underlyingid);


--
-- Name: digital_option fk_digital_ratecurve; Type: FK CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.digital_option
    ADD CONSTRAINT fk_digital_ratecurve FOREIGN KEY (ratecurveid) REFERENCES public.ratecurve(ratecurveid);


--
-- Name: digital_option fk_digital_underlying; Type: FK CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.digital_option
    ADD CONSTRAINT fk_digital_underlying FOREIGN KEY (underlyingid) REFERENCES public.underlying(underlyingid);


--
-- Name: option fk_option_underlying; Type: FK CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.option
    ADD CONSTRAINT fk_option_underlying FOREIGN KEY (underlyingid) REFERENCES public.underlying(underlyingid);


--
-- Name: range_option fk_range_underlying; Type: FK CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.range_option
    ADD CONSTRAINT fk_range_underlying FOREIGN KEY (underlyingid) REFERENCES public.underlying(underlyingid);


--
-- Name: asian_option fk_ratecurve; Type: FK CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.asian_option
    ADD CONSTRAINT fk_ratecurve FOREIGN KEY (ratecurveid) REFERENCES public.ratecurve(ratecurveid);


--
-- Name: trade fk_trade_market; Type: FK CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.trade
    ADD CONSTRAINT fk_trade_market FOREIGN KEY (marketid) REFERENCES public.market(marketid) ON DELETE CASCADE;


--
-- Name: trade fk_trade_underlying; Type: FK CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.trade
    ADD CONSTRAINT fk_trade_underlying FOREIGN KEY (underlyingid) REFERENCES public.underlying(underlyingid) ON DELETE CASCADE;


--
-- Name: asian_option fk_underlying; Type: FK CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.asian_option
    ADD CONSTRAINT fk_underlying FOREIGN KEY (underlyingid) REFERENCES public.underlying(underlyingid);


--
-- Name: historicalprice historicalprice_underlyingid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.historicalprice
    ADD CONSTRAINT historicalprice_underlyingid_fkey FOREIGN KEY (underlyingid) REFERENCES public.underlying(underlyingid);


--
-- Name: market market_exchangeid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.market
    ADD CONSTRAINT market_exchangeid_fkey FOREIGN KEY (exchangeid) REFERENCES public.exchange(exchangeid);


--
-- Name: option option_ratecurveid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.option
    ADD CONSTRAINT option_ratecurveid_fkey FOREIGN KEY (ratecurveid) REFERENCES public.ratecurve(ratecurveid);


--
-- Name: ratepoint ratepoint_ratecurveid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.ratepoint
    ADD CONSTRAINT ratepoint_ratecurveid_fkey FOREIGN KEY (ratecurveid) REFERENCES public.ratecurve(ratecurveid);


--
-- Name: underlyingmarket underlyingmarket_marketid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.underlyingmarket
    ADD CONSTRAINT underlyingmarket_marketid_fkey FOREIGN KEY (marketid) REFERENCES public.market(marketid);


--
-- Name: underlyingmarket underlyingmarket_underlyingid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: karankapoor
--

ALTER TABLE ONLY public.underlyingmarket
    ADD CONSTRAINT underlyingmarket_underlyingid_fkey FOREIGN KEY (underlyingid) REFERENCES public.underlying(underlyingid);


--
-- PostgreSQL database dump complete
--

\unrestrict DfCR84ZJv1e5qBsLZGD7srgLony7pvwAx5JGpvYI7VXHRzmrgHlc70VdwjW4FhQ

