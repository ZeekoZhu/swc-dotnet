#![deny(warnings)]

use std::panic::{catch_unwind, AssertUnwindSafe};

use anyhow::{anyhow, Error};
use swc_core::{
    base::{config::ErrorFormat, try_with_handler},
    common::{
        errors::Handler,
        sync::{Lrc},
        SourceMap,
    },
};


pub fn try_with<F, Ret>(
    cm: Lrc<SourceMap>,
    skip_filename: bool,
    _error_format: ErrorFormat,
    op: F,
) -> Result<Ret, Error>
    where
        F: FnOnce(&Handler) -> Result<Ret, Error>,
{
    try_with_handler(
        cm,
        swc_core::base::HandlerOpts {
            skip_filename,
            ..Default::default()
        },
        |handler| {
            //
            let result = catch_unwind(AssertUnwindSafe(|| op(handler)));

            let p = match result {
                Ok(v) => return v,
                Err(v) => v,
            };

            if let Some(s) = p.downcast_ref::<String>() {
                Err(anyhow!("failed to handle: {}", s))
            } else if let Some(s) = p.downcast_ref::<&str>() {
                Err(anyhow!("failed to handle: {}", s))
            } else {
                Err(anyhow!("failed to handle with unknown panic message"))
            }
        },
    )
}
