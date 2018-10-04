using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using SharpMapper;

namespace SharpMapper.Entity
{
    /// <summary>
    /// Classe che implementa le CRUD e la chiave primaria.
    /// La chiave primaria sul db è "id".
    /// </summary>
    public class BaseEntity
    {
        #region Members and properties

        private int? m_id;
        [Field("id", true)]
        public int? Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        #endregion Members and properties

        #region Public methods (CRUD)

        public virtual void Save()
        {
            IDbConnection t_conn = SharpMapperHelper.Instance.GetConnection();
            IDbTransaction t_trans = t_conn.BeginTransaction();

            try
            {
                if (this.m_id == null)
                    SharpMapperHelper.Instance.ExecuteInsertCommand(t_trans, this);
                else
                    SharpMapperHelper.Instance.ExecuteUpdateCommand(t_trans, this);
            }
            catch (Exception p_ex)
            {
                t_trans.Rollback();
                throw p_ex;
            }
            finally
            {
                t_conn.Close();
            }
        }

        public virtual T Find<T>(int p_id)
            where T : new()
        {
            IDbConnection t_conn = SharpMapperHelper.Instance.GetConnection();
            IDbTransaction t_trans = t_conn.BeginTransaction();

            try
            {
                T t_genericObject = SharpMapperHelper.Instance.FindByID<T>(t_trans, p_id);
                if (t_genericObject != null)
                    return t_genericObject;
            }
            catch (Exception p_ex)
            {
                t_trans.Rollback();
                throw p_ex;
            }
            finally
            {
                t_conn.Close();
            }

            return default(T);
        }

        public virtual T FindBySQL<T>(string p_sqlWhere)
            where T : new()
        {
            IDbConnection t_conn = SharpMapperHelper.Instance.GetConnection();
            IDbTransaction t_trans = t_conn.BeginTransaction();

            try
            {
                T t_genericObject = SharpMapperHelper.Instance.FindBySQL<T>(t_trans, p_sqlWhere);
                if (t_genericObject != null)
                    return t_genericObject;
            }
            catch (Exception p_ex)
            {
                t_trans.Rollback();
                throw p_ex;
            }
            finally
            {
                t_conn.Close();
            }

            return default(T);
        }

        public virtual void Delete(int p_id)
        {
            IDbConnection t_conn = SharpMapperHelper.Instance.GetConnection();
            IDbTransaction t_trans = t_conn.BeginTransaction();

            try
            {
                SharpMapperHelper.Instance.ExecuteDeleteCommand(t_trans, p_id);
            }
            catch (Exception p_ex)
            {
                t_trans.Rollback();
                throw p_ex;
            }
            finally
            {
                t_conn.Close();
            }
        }

        public virtual List<T> List<T>()
            where T : new()
        {
            IDbConnection t_conn = SharpMapperHelper.Instance.GetConnection();
            IDbTransaction t_trans = t_conn.BeginTransaction();

            try
            {
                List<T> t_genericObjectList = SharpMapperHelper.Instance.ExecuteList<T>(t_trans);
                if (t_genericObjectList != null)
                    return t_genericObjectList;
            }
            catch (Exception p_ex)
            {
                t_trans.Rollback();
                throw p_ex;
            }
            finally
            {
                t_conn.Close();
            }

            return default(List<T>);
        }

        #endregion Public methods (CRUD)
    }
}