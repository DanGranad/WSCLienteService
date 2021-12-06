using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSGCliente.Entities.BindingModel.Intermediarios;

namespace WSGCliente.Entities.BindingModel
{
    public class ClientBindingModel
    {
        /* Atributos Validacion Masiva */
        public string P_SAPP { get; set; }
        public string P_SNOPROCESO { get; set; }
        public Int64 P_NNUMREG { get; set; }
        public string P_SFILENAME { get; set; }
        /* Fin */
        public string P_NROW { get; set; }
        public string P_CodAplicacion { get; set; }
        public string P_TIPO_VAL { get; set; }
        public string P_TipOper { get; set; }
        public string P_NUSERCODE { get; set; }
        public string P_NIDDOC_TYPE { get; set; }
        public string P_SIDDOC { get; set; }
        public string P_SFIRSTNAME { get; set; }
        public string P_SLASTNAME { get; set; }
        public string P_SLASTNAME2 { get; set; }
        public string P_SLEGALNAME { get; set; }
        public string P_SSEXCLIEN { get; set; }
        public string P_NINCAPACITY { get; set; }
        public string P_NINCAP_COD { get; set; }
        public string P_DBIRTHDAT { get; set; }
        public string P_DINCAPACITY { get; set; }
        public string P_NHEALTH_ORG { get; set; }
        public string P_DDEATHDAT { get; set; }
        public string P_DWEDD { get; set; }
        public string P_SACCOUNT_IN { get; set; }
        public string P_NINVOICING { get; set; }
        public string P_NSPECIALITY { get; set; }
        public string P_NCIVILSTA { get; set; }
        public string P_DAPROBDATE { get; set; }
        public string P_SBLOCKADE { get; set; }
        public string P_NCLASS { get; set; }
        public string P_DDRIVERDAT { get; set; }
        public string P_NHEIGHT { get; set; }
        public string P_NHOUSE_TYPE { get; set; }
        public string P_SLICENSE { get; set; }
        public string P_NOFFICE { get; set; }
        public string P_NNOTENUM { get; set; }
        public string P_NQ_CARS { get; set; }
        public string P_NQ_CHILD { get; set; }
        public string P_NRATE { get; set; }
        public string P_STAX_CODE { get; set; }
        public string P_SSMOKING { get; set; }
        public string P_SCUIT { get; set; }
        public string P_NTITLE { get; set; }
        public string P_NOCCUPATION { get; set; }
        public string P_NWEIGHT { get; set; }
        public string P_SAUTO_CHAR { get; set; }
        public string P_SCREDIT_CARD { get; set; }
        public string P_NECONOMIC_L { get; set; }
        public string P_NEMPL_QUA { get; set; }
        public string P_NIMAGENUM { get; set; }
        public string P_NAREA { get; set; }
        public string P_DDRIVEXPDAT { get; set; }
        public string P_NTYPDRIVER { get; set; }
        public string P_NDISABILITY { get; set; }
        public string P_NLIMITDRIV { get; set; }
        public string P_NAFP { get; set; }
        public string P_SBILL_IND { get; set; }
        public string P_NNATIONALITY { get; set; }
        public string P_SDIGIT { get; set; }
        public string P_DRETIREMENT { get; set; }
        public string P_DINDEPENDANT { get; set; }
        public string P_DDEPENDANT { get; set; }
        public string P_NMAILINGPREF { get; set; }
        public string P_NLANGUAGE { get; set; }
        public string P_SLEFTHANDED { get; set; }
        public string P_SBLOCKLAFT { get; set; }
        public string P_SISCLIENT_IND { get; set; }
        public string P_SISRENIEC_IND { get; set; }
        public string P_NCLIENT_SEG { get; set; }
        public string P_SPOLIZA_ELECT_IND { get; set; }
        public string P_TI_DOC_SUSTENT { get; set; }
        public string P_NU_DOC_SUSTENT { get; set; }
        public string P_COD_UBIG_DEP_NAC { get; set; }
        public string P_COD_UBIG_PROV_NAC { get; set; }
        public string P_COD_UBIG_DIST_NAC { get; set; }
        public string P_DEPARTAMENTO_NACIMIENTO { get; set; }
        public string P_PROVINCIA_NACIMIENTO { get; set; }
        public string P_DISTRITO_NACIMIENTO { get; set; }
        public string P_NOMBRE_PADRE { get; set; }
        public string P_NOMBRE_MADRE { get; set; }
        public string P_FECHA_INSC { get; set; }
        public string P_FECHA_EXPEDICION { get; set; }
        public string P_CONSTANCIA_VOTACION { get; set; }
        public string P_APELLIDO_CASADA { get; set; }
        public string P_SDIG_VERIFICACION { get; set; }
        public string P_SPROTEG_DATOS_IND { get; set; }
        public string P_SGRADO_INSTRUCCION { get; set; }        
        public string P_COD_CUSPP { get; set; }
        public string P_FOTO_RENIEC { get; set; }
        public string P_FIRMA_RENIEC { get; set; }
        public string P_SSISTEMA { get; set; }
        public string P_NROL { get; set; }
        public string P_SISSEACSA_IND { get; set; }
        public string P_SISCLIENT_GBD { get; set; }
        public string P_CONSUMESERV_IND { get; set; }
        public string P_SISCLIENT_CRITICO { get; set; }
        public string P_SBAJAMAIL_IND { get; set; }
        public string P_SREGIST { get; set; }
        public string P_SREGISTCIIU { get; set; }

//Intermediarios y contrantes
        
        public string P_DFE_ANIVERSARIO { get; set; }
        public string P_SISCLIENT_CORREDOR { get; set; }
        public string P_SISCLIENT_CONTRATANTE { get; set; }
        public string P_NIND_CORREDOR { get; set; }
        public string P_SNAME_COMERCIAL { get; set; }
        public string P_NCANT_TRABAJADORES { get; set; }
        public string P_NNUMERO_USUARIO { get; set; }
        public string P_SCOD_SBS { get; set; }
        public string P_SIND_CONSENTI_CLAUS { get; set; }
        public string P_SPARTIDA_ELECTRONICA { get; set; }  
        public string P_NID_SECTOR_EMPRE { get; set; }
        public string P_NID_AREA_RESPONSABLE { get; set; }
        public string P_DFECHA_RECEP_FACTU { get; set; }
        public string P_SIND_VENTA_COMERCIAL { get; set; }
        public string P_SLIDER_CONSORCIO { get; set; }

        //hcama@mg 03.06.2021 ini
        public string P_TIPO_BUSQUEDA { get; set; }
        public string P_VALOR_BUSQUEDA { get; set; }
        public Int64 P_NBRANCH { get; set; }
        public Int64 P_NPRODUCT { get; set; }
        public Int64 P_NPOLICY { get; set; }
        public Int64 P_NCERTIF { get; set; }
        //hcama@mg 03.06.2021 fin


        public List<DocumentosBindingModel> ElistDocumentClient { get; set; }
        public List<InfoBancariaBindingModel> ElistInfoBancariaClient { get; set; }



        public List<AddressBindingModel> EListAddresClient { get; set; }
        public List<PhoneBindingModel> EListPhoneClient { get; set; }
        public List<EmailBindingModel> EListEmailClient { get; set; }
        public List<ContactBindingModel> EListContactClient { get; set; }
        public List<CiiuBindingModel> EListCIIUClient { get; set; }


    }
}
